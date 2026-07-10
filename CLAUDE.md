# Setup rules

These are the conventions for working in this repo. Follow them by default; deviate only with reason.

## Tooling

- Toolchain is pinned in [.mise.toml](.mise.toml). Run `mise install` before doing anything else.
- Run all repo commands through `mise run <task>`, not ad-hoc shells. If a recurring command doesn't have a task, add one to `.mise.toml` instead of documenting it elsewhere.
- Scripts under `tools/` are bash (`*.sh`), invoked via `bash tools/<name>.sh` from mise tasks (see `clone-project`). On Windows, bash comes from Git Bash. Don't introduce other scripting languages for tooling.

## Code style

- Format everything via `mise run format` — csharpier for C#, Prettier for TS/CSS/JSON. CI gate is `mise run format:check`. ESLint runs separately: `mise run client:lint`.
- No comments unless the *why* is non-obvious. Don't restate what the code already says.
- Nullable reference types are **disabled** in every project (`<Nullable>disable</Nullable>`). Don't write `?` on reference type declarations (parameters, properties, return types, fields). Value type `?` (e.g., `int?`, `DateTime?`) is fine — that's `Nullable<T>`, not the reference-type annotation. If a new project is added, set `<Nullable>disable</Nullable>` to match.
- `Aesys.Web` and `aesys.web` are placeholder names. To start a new project from this template, use `mise run clone-project <NewName> <DestinationPath>` — never hand-edit folder/csproj names. It copies the tracked tree (via `git archive HEAD`) into the destination, rewrites the placeholder, and `git init`s a fresh repo there.

## Skills

Specialised skills auto-load for Umbraco work. The model picks by description; this index is for humans:

- [component-developer](.claude/skills/component-developer/SKILL.md) — entry-point orchestrator for adding a new component. Picks the bucket (pure UI vs page-scoped block vs shared block vs site-wide composition) and delegates to the four specialised skills below.
- [usync-author](.claude/skills/usync-author/SKILL.md) — code-first DocumentType `.config` mechanics, GUID uniqueness, rename round-trip, bundler.
- [umbraco-viewcomponent](.claude/skills/umbraco-viewcomponent/SKILL.md) — Razor render: co-located ViewComponent + ViewModel record, namespace-shadow workaround, partial discovery.
- [umbraco-datatypes](.claude/skills/umbraco-datatypes/SKILL.md) — picking/creating DataTypes; index of the editors tracked under `Aesys.Web/uSync/v17/DataTypes/`.
- [umbraco-blocks](.claude/skills/umbraco-blocks/SKILL.md) — Block List/Grid/single composition from IsElement doctypes, dispatch through `Aesys.Web/Views/Partials/`.

## Database

- Default DB is SQLite, configured in [appsettings.json](Aesys.Web/appsettings.json) via `umbracoDbDSN` + `|DataDirectory|`.
- The DB file lives at `./data/Umbraco.sqlite.db` (host bind mount from both compose files). `./data/` is gitignored — never commit it.
- Resetting the DB = stopping the stack and deleting `./data/Umbraco.sqlite.db*` (3 files: `.db`, `.db-shm`, `.db-wal`).
- **NEVER drop, delete, reset, overwrite, or otherwise destroy the database without an explicit, in-the-moment instruction from the user to do exactly that.** The SQLite file holds real authored content (pages, media, dictionary entries) that is **not** reproducible from the code-first schema — uSync imports doctypes/datatypes, not content. The local dev DB lives at `Aesys.Web/umbraco/Data/Umbraco.sqlite.db` (non-Docker `mise run dev`); the Docker path is the `./data/` bind mount. Do not assume a "missing" DB — if you don't see it in one location, check the other before concluding it's a fresh install. Deleting it to "fix" a boot error, regenerate models, or get a clean slate is prohibited.
- SQLite means single-instance only. If horizontal scaling becomes a requirement, switch the connection string to SQL Server / PostgreSQL before scaling `web`.

### First-boot install (dev only)

- [appsettings.Development.json](Aesys.Web/appsettings.Development.json) enables `Umbraco:CMS:Unattended:InstallUnattended` and seeds an admin user. This is required: with a `|DataDirectory|` SQLite connection string pre-configured, Umbraco's runtime state machine routes a missing DB to `BootFailed` (reason `InstallMissingDatabase`) instead of showing the install wizard — so unattended install is the only way to bootstrap dev.
- Dev admin: `admin@local` / `LocalDev1234!`. Change before exposing the dev container off-localhost.
- Production ([appsettings.json](Aesys.Web/appsettings.json)) intentionally has **no** unattended config — first prod boot must be installed deliberately (env-var override or manual config).

## Docker

- `docker-compose.yml` is the base. `docker-compose.override.yml` is picked up automatically and switches to the `dev` build target with `dotnet watch` + source bind mount.
- The data directory uses a **host bind mount** (`./data`) in both files — this is intentional so the SQLite file is inspectable. Logs and media remain named volumes.
- Don't bake runtime artefacts (DB, logs, media, schemas) into the image — they're already excluded via [.dockerignore](.dockerignore).

## Client assets (Vite + Tailwind v4 + SCSS)

Client-side build is Vite-driven, owned entirely by [Aesys.Web/](Aesys.Web/). Output lands in `Aesys.Web/wwwroot/dist/` (gitignored, image-built).

`main.css` stays as plain CSS — it's the Tailwind v4 entry, and Tailwind's directives (`@import 'tailwindcss'`, `@source`, `@theme`, `@apply`) must reach the Tailwind Vite plugin unprocessed. Everything else — tokens, base, typography, component partials — is `.scss` (compiled by `sass-embedded` before Vite hands the result to Tailwind). Use SCSS features freely in partials; just don't put Tailwind directives in them.

### Folder layout

- [Aesys.Web/Client/](Aesys.Web/Client/) — global concerns
  - `main.ts` — entry; imports `main.css` and glob-imports every co-located component `.ts` and `.scss`
  - `main.css` — Tailwind v4 entry + `@source` scans (`.cshtml`, `.ts`, `.cs` in Aesys.Core) + token/base/typography `@import`s (which resolve to `.scss` partials via Vite)
  - `lib/component.ts` — `defineComponent(selector, init)` idempotent DOM-binding primitive
  - `tokens/`, `base/`, `typography/`, `assets/`, `fonts/` — design-system globals (SCSS)
- [Aesys.Web/Views/Shared/Components/{Name}/](Aesys.Web/Views/Shared/Components/Header/) — co-located per-component `*.ts` / `*.scss` next to `Default.cshtml`. **Just drop a file in; no registration.** Vite picks it up via `import.meta.glob('../Views/**/*.{ts,scss}', { eager: true })` in `main.ts`.
- [Aesys.Web/TagHelpers/](Aesys.Web/TagHelpers/) — `<vite-asset>` tag helper + `ViteManifest` singleton
- [Aesys.Web/Extensions/](Aesys.Web/Extensions/) — `@Html.Cn(...)` (backed by TailwindMerge.NET) for conflict-resolving class composition
- `Aesys.Core/<bucket>/<Name>/<Name>Variants.cs` — **optional** cva-style variants helper; **only** when a component has actual reused variant logic (theme/size/state branches). Default to writing classes inline in the `.cshtml` (Tailwind scans `.cshtml` too). When a helper is warranted, its class strings are scanned by Tailwind via `@source "../../Aesys.Core/**/*.cs"`. Canonical examples of components that earn one: [HeaderVariants.cs](Aesys.Core/Compositions/Header/HeaderVariants.cs) (composition) and [ButtonVariants.cs](Aesys.Core/Components/UI/Button/ButtonVariants.cs) (pure-UI). See `### Authoring conventions` for the rule.

### Dev loop

`mise run dev` is a **single command** that runs Vite + dotnet watch concurrently in one terminal (via the `concurrently` npm dev dep). HMR works for CSS/TS; Razor changes still trigger a full reload via dotnet watch. Escape hatches: `mise run client:dev` and `mise run dotnet:watch` for two-terminal mode.

`<vite-asset entry="Client/main.ts" />` in [_Layout.cshtml](Aesys.Web/Views/_Layout.cshtml) emits dev-server URLs in Development and hashed manifest paths in Production (read once on startup from `wwwroot/dist/.vite/manifest.json`).

### Authoring conventions

- Razor classes: `class="@Html.Cn(HeaderVariants.Base, isOpen ? "bg-aesys-100" : "")"` — `Cn` calls `TwMerge.Merge` so conflicting classes resolve correctly (`px-4 px-6` → `px-6`).
- Variants: **inline classes in the `.cshtml` are the default.** Tailwind scans `.cshtml`, so static (even theme-conditional one-off) classes belong there — `class="@(theme == "dark" ? "bg-aesys-800 text-white" : "bg-surface-light")"` is fine inline. Only extract a `public static class XxxVariants` next to the ViewComponent when there is **actual** reused variant logic worth a helper: the same theme/size/state branch is needed in multiple places, or the branch table is large enough that inlining hurts readability — as [HeaderVariants.cs](Aesys.Core/Compositions/Header/HeaderVariants.cs) and [ButtonVariants.cs](Aesys.Core/Components/UI/Button/ButtonVariants.cs) do. **Never add a Variants file just because a component exists** — no boilerplate Variants class for its own sake.
- Component scripts: `defineComponent('[data-component="xxx"]', el => { ... })` from `@/lib/component`. Razor opts in by adding `data-component="xxx"` to the root element. The `__inited` guard is idempotent — safe for Umbraco backoffice DOM swaps.
- Path aliases: `@/...` → `Aesys.Web/Client/`, `@views/...` → `Aesys.Web/Views/`.
- New design tokens go in [Aesys.Web/Client/tokens/tokens.scss](Aesys.Web/Client/tokens/tokens.scss) via Tailwind v4's `@theme` directive (`--color-*`, `--font-*`, `--radius-*`). SCSS passes `@theme` through untouched, so Tailwind still reads it.

### Adding dependencies

```
cd Aesys.Web
npm install <pkg> --save        # runtime — joins the bundle
npm install <pkg> --save-dev    # build-time only
```

Commit both `package.json` AND `package-lock.json` — Docker uses `npm ci` against the lockfile. Prefer ESM-only packages with named imports for tree-shaking; CommonJS deps don't tree-shake well.

### Production build

`mise run client:build` runs `vite build` → emits hashed `wwwroot/dist/assets/*.{js,css,map}` + a separate `vendor-*.js` chunk (so vendor cache survives app-only changes) + `.vite/manifest.json`. Source maps are emitted hidden (no `//# sourceMappingURL=` reference; available for error-tracker symbolication).

Docker prod build has a dedicated `client` stage (`node:22-alpine`) that runs `npm ci && npm run build`; the `build` stage `COPY --from=client` brings `wwwroot/dist/` in before `dotnet publish`. No node binaries in the runtime image.

### Recommended VSCode extensions

- `dbaeumer.vscode-eslint` — ESLint w/ format-on-save
- `esbenp.prettier-vscode` — Prettier as default formatter
- `bradlc.vscode-tailwindcss` — Tailwind v4 IntelliSense

## Umbraco specifics

- `UpgradeUnattended` is on, so migrations apply on boot.
- Razor compile-on-build/publish is disabled by design (see comment in the csproj). Don't re-enable without understanding the InMemoryAuto ModelsMode implication.
- Generated schema files (`appsettings-schema*.json`, `umbraco-package-schema.json`) are gitignored — they regenerate on build.
- **ModelsBuilder-generated files (`Aesys.Core/Generated/*.generated.cs`) are off-limits**. Never rename, move, or hand-edit them. They're owned by the generator — overwritten on every regen (SourceCodeAuto runs on every doctype save in dev). They are tracked in git (no gitignore) so PRs show the model deltas. If a doctype rename breaks compile transiently, fix it by changing the source `.config` and waiting for MB to regen — don't shortcut by editing the generated file.

### New-model bootstrap ordering (IMPORTANT)

> **STOP — order is not optional.** When you add or change a doctype/property that C# will consume, do **NOT** write or edit any C# that references the new/changed model member until the app has booted once and regenerated the model. The order is strictly: **(1) edit `.config` → (2) `mise run usync:bundle` → (3) BOOT the app (`mise run dev`) and wait for the regenerated `*.generated.cs` → (4) ONLY THEN write the model-consuming C#.** Writing the C# first breaks the build, which prevents the app from booting, which prevents the regen — a deadlock you then have to back out of (revert the C#, boot, restore it). "I'm already in the file, I'll just write it now" is the exact trap. If model-consuming C# already exists and blocks the build, move it aside (or `git stash`/revert it), boot to regen, then restore — see step 3 below.

`ModelsMode` is `SourceCodeAuto` (dev): generated models in `Aesys.Core/Generated/` are written **by the running app**, not by `dotnet build`. So C# that references a *new or changed* model member (a new doctype's `Models.X`, a newly-added property like `HomePage.Socials`, a new element model) **cannot compile until the app has run once and regenerated the models**. Build-first fails with `CS0234`/`CS0246` — a chicken-and-egg.

Follow this order whenever you add or change a doctype/property that your C# will consume:

1. Author/edit the `.config` source under `Aesys.Core/`.
2. `mise run usync:bundle` — flatten sources into `uSync/v17/ContentTypes/`.
3. **Run the app via `mise run dev`** so uSync imports the schema and MB regenerates `*.generated.cs`. **Use `mise run dev` (dotnet watch), not a bare `dotnet run`** — the watch-driven cycle is what reliably writes the regenerated models to `Aesys.Core/Generated/`. A uSync startup import alone does **not** force the `SourceCodeAuto` write; if models still don't appear, a content-type **Save** in the backoffice forces a full regen. If existing model-consuming C# (other ViewComponents) blocks the build so the app can't boot, temporarily move those `.cs` aside (Razor views are runtime-compiled and don't block the build), boot once, then restore them.
4. **Then** write/finish the C# (ViewComponents, etc.) that references the new members and rebuild — `dotnet watch` picks it up green. Note: when a regen changes a generated model's **base class or interface list** (e.g. a doctype starts composing a mixin), `dotnet watch` reports a hot-reload **rude edit** (`ENC0014: Updating the base class … requires restarting`) and prompts to restart — answer yes / restart.

A fresh clone with no `data/` DB has no content models yet; the first `mise run dev` performs the unattended install and the initial generation. Committed generated models must match the configured `Umbraco:CMS:ModelsBuilder:ModelsNamespace` (`Aesys.Core.Models`) — stale files under a different namespace won't satisfy `Models.X` references until regenerated.

**Composition interfaces (`IXxx`) are generated lazily.** ModelsBuilder emits the interface for a composition **only once another doctype actually composes it**. So a ViewComponent written as `Invoke(IXxx source)` won't compile until a consumer exists and the app has regenerated. For a composition with no consumer yet, either take the concrete `Models.Xxx` for now, or add the consumer first.

### Compositions consumed by element blocks must be `IsElement=true` (IMPORTANT)

ModelsBuilder **refuses to generate any models** if an element type (`IsElement=true`) composes a non-element type (`IsElement=false`). The whole generation aborts with:

```
Cannot generate model for type 'X' because it is an element type, but it is composed of 'Y' which is not.
```

So a composition's `IsElement` flag depends on **who consumes it**, not on the fact that it lives under `Compositions/`:

- A composition consumed only by **page/document types** (`IsElement=false`) — e.g. `Header`, `Footer`, `GlobalSettings` on `HomePage` — is itself `IsElement=false` (the classic site-wide mixin).
- A composition consumed by **element blocks** (`IsElement=true`) — e.g. a `Section` or `IntroText` mixin composed by `HeroBanner` / `TextWithImage` — **must itself be `IsElement=true`** (a content-bearing mixin shared by blocks is an element composition).

If a mixin is composed by both a page and an element block, make it `IsElement=true` (elements are the stricter constraint); a page can still compose an element type.

### Block editor labels use Umbraco Flavored Markdown (UFM), not `{{angular}}`

The pre-14 AngularJS backoffice rendered block-list/grid labels with `{{propertyAlias}}`. **That syntax is dead in Umbraco 14+ (incl. 17)** — it renders literally. The new backoffice uses **UFM**: `{=propertyAlias}` (shorthand for `{umbValue: propertyAlias}`), set in the Block List/Grid DataType's `"label"` JSON. Useful filters:

- `{=title | fallback:Untitled}` — default when empty
- `{=text | truncate:50}` / `{=text | wordLimit:5}` — length limits
- `{=body | stripHtml}` — **required** for a richtext property; UFM won't render raw richtext markup otherwise.

### Page templates — managed in code, NEVER put `Layout` in a template view

- **Never add a `Layout = "..."` line to a page template view** (`Aesys.Web/Views/{Alias}.cshtml`). The default layout is set once in [Aesys.Web/Views/_ViewStart.cshtml](Aesys.Web/Views/_ViewStart.cshtml). **Why:** Umbraco's template create parses a view's `Layout = "X"` as a *master-template alias* and fails with `MasterTemplateNotFound` (our `_Layout` is a plain MVC layout, not an Umbraco master). uSync 17.3.2 also can't import templates on Umbraco 17 at all, so templates are not uSync-managed.
- **Templates are created in code, not uSync.** uSync's `TemplateHandler` is disabled (both appsettings). On startup [EnsurePageTemplatesHandler](Aesys.Core/Notifications/EnsurePageTemplatesHandler.cs) (registered via `RegisterCore` in [Aesys.Core/Extensions/](Aesys.Core/Extensions/)) iterates doc types and, for each one that has a `Views/{Alias}.cshtml`, creates a matching Template from that view and sets it as the doc type's default/allowed template. It's idempotent (no churn).
- **To add a page template:** drop `Aesys.Web/Views/{Alias}.cshtml` (Alias = PascalCase of the doc type alias, e.g. `homePage` → `HomePage.cshtml`) with **no `Layout` line**, and don't add anything under uSync `Templates/`. The checker wires it on next boot.
- All Core service/notification registrations live in the single `RegisterCore(this IUmbracoBuilder)` extension, called from [Program.cs](Aesys.Web/Program.cs).

### Services — DI classes live in `Aesys.Core/Services/` and are named `*Service`

- **Any class registered for DI** (anything you `builder.Services.Add*<...>()` in `RegisterCore`) is a **service**: put it in `Aesys.Core/Services/<Name>Service.cs` with namespace `Aesys.Core.Services`, name the class `<Name>Service`, and give it an `I<Name>Service` interface registered against it. Examples: [ContactEmailService.cs](Aesys.Core/Services/ContactEmailService.cs), [BlogListingService.cs](Aesys.Core/Services/BlogListingService.cs).
- **Name by what it does, not by mechanism** — `ContactEmailService`, not `ContactMailer`/`ContactHelper`/`ContactManager`. The `*Service` suffix is the signal "this is DI-registered, lives under `Services/`."
- **Don't co-locate services with components.** They're cross-cutting; a component folder is for that component's `.config`/ViewComponent/Variants/partial, not its services. A service may still reference a component's view models (e.g. `BlogListingService` returns `BlogCardsViewModel`) — that's a one-way `using`, not co-location.

### Component taxonomy

Four buckets under `Aesys.Core/` — pick by **scope** and **whether Umbraco backs it**. The [component-developer skill](.claude/skills/component-developer/SKILL.md) owns the decision tree and delegates the mechanical work.

| Bucket | Folder | Umbraco-backed? | `.config`? | DataTypes? | Block-listable? | Example |
|---|---|---|---|---|---|---|
| **Pure UI** | `Components/UI/<Name>/` | No | No | No | No | [Button](Aesys.Core/Components/UI/Button/) — invoked inline by other components |
| **Page-scoped block** | `Components/<Page>/<Name>/` | Yes, `IsElement=true` | Yes | Yes | Yes | [HeroBanner](Aesys.Core/Components/HomePage/HeroBanner/) under HomePage |
| **Shared block** | `Shared/<Name>/` | Yes, `IsElement=true` | Yes | Yes | Yes (any page's block-list) | *(none yet — first cross-page reusable block goes here)* |
| **Site-wide composition** | `Compositions/<Name>/` | Yes, `IsElement=false` mixin¹ | Yes | Yes | No | [Header](Aesys.Core/Compositions/Header/), [Footer](Aesys.Core/Compositions/Footer/), [GlobalSettings](Aesys.Core/Compositions/GlobalSettings/) |

¹ `IsElement=false` only when consumed by page/document types. A composition consumed by **element blocks** (e.g. a `Section`/`IntroText` mixin composed by `HeroBanner`/`TextWithImage`) **must be `IsElement=true`** — see `### Compositions consumed by element blocks must be IsElement=true` under `## Umbraco specifics`.

Rules:
- The folder path under `Aesys.Core/` determines the C# namespace (`Aesys.Core.Components.UI.Button`, `Aesys.Core.Components.HomePage.HeroBanner`, `Aesys.Core.Shared.<Name>`, `Aesys.Core.Compositions.Header`).
- Razor partials always live at `Aesys.Web/Views/Shared/Components/<Name>/Default.cshtml` regardless of bucket — ViewComponent discovery is by class name, not source folder.
- Pure-UI components have **no** `.config`, **no** entry in any block editor DataType, and are invoked inline (`Component.InvokeAsync("Button", new { ... })`).
- Compositions take an **interface** in `Invoke(IHeader source)`; `Components/<Page>/<Name>/` and `Shared/<Name>/` element types take the **class** as `Invoke(Models.X source)`; pure UI takes plain primitives. (Composition interfaces are generated lazily — see the bootstrap-ordering note.)
- **Wrap section chrome with the `<section-block>` tag helper, not hand-rolled `<section>` markup.** It renders the standard outer chrome — dark surface + full-bleed background image + scrim when a `bg-image` is set, light section + gray bottom border when not — plus the inner `wrapper` container, and accepts Razor children (a ViewComponent can't). Pass `bg-image="@(Model.Background?.Url())"`; arbitrary attributes (`id`, `data-component`, `aria-*`) and `class` pass through (`class` is TwMerge'd). It owns the first-block header offset. Canonical consumers: [HeroBanner](Aesys.Web/Views/Shared/Components/HeroBanner/Default.cshtml), [TextWithImage](Aesys.Web/Views/Shared/Components/TextWithImage/Default.cshtml).
- **Design-system radius scale is overridden** in [tokens.scss](Aesys.Web/Client/tokens/tokens.scss) (`--radius-sm/md/lg/xl` only). Stick to `rounded-sm/md/lg/xl`; raw Tailwind steps like `rounded-2xl`/`rounded-3xl` are **unmapped** and won't render the brand radius.

## uSync

uSync folder is `Aesys.Web/uSync/v17/`. Behaviour splits by environment.

- **Dev** ([appsettings.Development.json](Aesys.Web/appsettings.Development.json)): `ImportAtStartup: "All"` + `ExportOnSave: "Settings"`. `ContentHandler` and `MediaHandler` are disabled. `ContentTypeHandler` and `DictionaryHandler` are set to `Actions: "Import"` — they apply on startup but never write back, because doctypes and dictionary entries are code-first (authored in source under [Aesys.Core/](Aesys.Core/), bundled into the folder via `mise run usync:bundle`). Don't re-enable content/media in dev.
- **Prod** ([appsettings.json](Aesys.Web/appsettings.json)): `ImportAtStartup: "None"` + `ExportOnSave: "All"`. Every backoffice save (content, media, dictionary, schema) writes uSync XML to disk — but that disk is ephemeral in prod (see `Prod uSync is image-baked` below). Operator triggers import manually after each deploy.

Tracked vs gitignored in `Aesys.Web/uSync/v17/`:
- Tracked: `DataTypes/`, `Languages/`, `MediaTypes/`, `MemberTypes/`, `RelationTypes/`, `Templates/`.
- Gitignored: `ContentTypes/` (DocumentTypes — code-first), `Dictionary/` (code-first starting items).

Code-first authoring:
- Use the [usync-author skill](.claude/skills/usync-author/SKILL.md). It enforces a mandatory GUID-uniqueness check before assigning any `Key` to a new DocumentType or Dictionary entry.
- Source files live under [Aesys.Core/](Aesys.Core/) organised as `{Components,Compositions,Shared,Pages}/<Name>/<name>.config` — see the [usync-author skill](.claude/skills/usync-author/SKILL.md) for the layout rules and `### Component taxonomy` above for which bucket to pick. Pure-UI components under `Components/UI/` have no `.config` and are out of scope for uSync.
- `mise run usync:bundle` ([tools/usync-bundle.sh](tools/usync-bundle.sh)) wipes both `Aesys.Web/uSync/v17/ContentTypes/` and `Aesys.Web/uSync/v17/Dictionary/` and flat-copies every `*.config` under `Aesys.Core/`, **routed by folder**: files under a `Dictionary/` folder go to `Dictionary/`, everything else to `ContentTypes/` (so source deletes propagate). Run after every doctype or dictionary change. See `## Localization (i18n)` and the [usync-author skill](.claude/skills/usync-author/SKILL.md)'s `## Dictionary authoring` for the dictionary XML schema and key convention.

Prod uSync is **image-baked, not volume-mounted.** The Docker image ships a complete `uSync/v17/` — the tracked handlers (`DataTypes/`, `Languages/`, `MediaTypes/`, …) plus the code-first `ContentTypes/` + `Dictionary/` generated by the Dockerfile `usync-bundle` stage from `Aesys.Core/`. There is **no `./usync` bind mount** (it was removed) — the operator triggers import manually after a deploy and Umbraco reads the schema straight from the image.

- **Runtime captures are ephemeral.** `ExportOnSave: "All"` still writes uSync XML on every backoffice save, but it writes into the container's own filesystem (`/app/uSync/v17/`), which is **lost on container recreation**. Authored *content* is safe — it lives in the SQLite DB on the persistent `umbraco-data` volume — but uSync *file* captures of prod-side changes are not persisted. If you need them for env replication, `docker cp` them out of the running container before recreating it. (If prod never captures back, consider setting `ExportOnSave: "None"` there — follow-up, not done here.)
- **Schema updates ship in the next image.** Edit a `.config` under `Aesys.Core/`, rebuild, and the new image carries the regenerated `ContentTypes/`/`Dictionary/` automatically — no host-side seeding or merge step. The clean-checkout guarantee is why the bundler runs *inside* the build (CI does a fresh `git archive`/checkout where these folders don't exist).

## What not to commit

- `./data/` — runtime DB and Umbraco data
- `*.sqlite.db`, `*.sqlite.db-shm`, `*.sqlite.db-wal`
- `Aesys.Web/umbraco/Logs/` and `Aesys.Web/wwwroot/media/`
- `Aesys.Web/uSync/v17/ContentTypes/` and `Aesys.Web/uSync/v17/Dictionary/` — code-first artifacts (bundled from `Aesys.Core/`)
- `Aesys.Web/node_modules/`, `Aesys.Web/wwwroot/dist/`, `**/.vite/` — Vite client build output (rebuilt in Docker `client` stage)
- Secrets of any kind. There are no env-var files checked in; add them to `.mise.local.toml` (gitignored) if you need per-developer overrides.

## Localization (i18n)

Site text that isn't editor-authored content (UI chrome, form labels, validation messages, the contact email) is localized via **Umbraco Dictionary items**, code-first like DocumentTypes. Languages live in `Aesys.Web/uSync/v17/Languages/` (`sr` default, `en-US` secondary).

**Authoring (the write side).** Dictionary `.config` files live in a `Dictionary/` folder co-located with the owning component (e.g. `Aesys.Core/Shared/ContactForm/Dictionary/`, `Aesys.Core/Services/Dictionary/` for `Email.*`) plus a global `Aesys.Core/Dictionary/` for cross-cutting keys (`Common.*`, `Header.*`, `Footer.*`). The bundler routes by the `Dictionary/`-folder test. Keys are **dotted full paths** (`ContactForm.Submit`); the `<Parent>`/`Level` tree is backoffice-organizational only (lookups are by full ItemKey). Full schema, GUID-uniqueness, and the tree rules are in the [usync-author skill](.claude/skills/usync-author/SKILL.md)'s `## Dictionary authoring`.

**Reading (the read side).** One dictionary-backed store, three entry points:
- **`@Html.T("Key")`** (and `@Html.Tr("Key")` for string/attribute contexts) in Razor — [HtmlHelperLocalizationExtensions.cs](Aesys.Web/Extensions/HtmlHelperLocalizationExtensions.cs).
- **`ILocalizer`** in C# (controllers, services) — [Aesys.Core/Localization/](Aesys.Core/Localization/). Inject it; `localizer["Key"]` returns the value (or the key on a miss).
- **DataAnnotations** — `[Display(Name="Key")]` and `[Required(ErrorMessage="Key")]` etc. resolve from the dictionary automatically. This is wired in `RegisterCore` via `AddDataAnnotationsLocalization(o => o.DataAnnotationLocalizerProvider = (t, f) => f.Create(t))`, which routes every model's Display name and ErrorMessage through `DictionaryStringLocalizerFactory`. **Never set `ResourceType` on `[Display]`** — a null `ResourceType` is what routes it through the provider; setting it disables localization. Attributes carry **dictionary ItemKeys**, not literal text (attribute args must be compile-time constants, so the "static reference" is the key string).

All three funnel through `DictionaryStringLocalizer` (over Umbraco's `ICultureDictionary`), so there's one culture axis (`CurrentUICulture`, set per front-end request by Umbraco) and one fallback (miss → the key, rendered visibly). These are framework-abstraction implementations, so they live in `Aesys.Core/Localization/`, **not** `Aesys.Core/Services/` (the `*Service` convention in `### Services` is for domain services).

**Email culture caveat.** The contact email is built and sent server-side during the form POST (a surface controller, inside the Umbraco published route), so `CurrentUICulture` is the visitor's culture and `ILocalizer` resolves the email text correctly. If a send ever moves off the request thread (background/queue), pass the culture explicitly.

**Adding a language.** Drop a language `.config` in `Languages/` and add one `<Translation Language="...">` line per dictionary item. No code change.
