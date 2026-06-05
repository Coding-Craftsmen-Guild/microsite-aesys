---
name: umbraco-viewcomponent
description: Author Razor render code for Umbraco DocumentTypes AND pure-UI ViewComponents — co-located ViewComponent + ViewModel record under Aesys.Core/{Components,Compositions,Shared}/, the Models.X namespace-shadow workaround, the Compositions-take-interface rule, pure-UI takes-primitives variant, and where to put the Default.cshtml partial. Use when adding a render path for a doctype, mapping a generated PublishedContentModel to a strongly-typed view, wiring block-list items into ViewComponent dispatch, or authoring a pure-UI component like Button/Card.
---

# umbraco-viewcomponent

Razor render layer for code-first Umbraco DocumentTypes **and** pure-UI ViewComponents. The doctype `.config` and Aesys.Core source layout is owned by the [usync-author](../usync-author/SKILL.md) skill; the bucket-selection decision tree is owned by the [component-developer](../component-developer/SKILL.md) skill — this skill covers the C# + Razor side only.

## Co-location pattern

A ViewComponent and its ViewModel live in the same `.cs` file, in the same folder. For Umbraco-backed doctypes that's the same folder as the source `.config` (see the layout in [usync-author](../usync-author/SKILL.md#source-layout)). For **pure UI components** under `Aesys.Core/Components/UI/<Name>/`, there's no `.config` — the folder holds just the `<Name>ViewComponent.cs` plus an optional `<Name>Variants.cs`.

File convention:

- Filename: `<Name>ViewComponent.cs` (PascalCase).
- Namespace: `Aesys.Core.<Category>.<...>.<Name>` matching the folder path.
- Both **`<Name>ViewModel`** (a sealed `record` of primitives + Umbraco render-ready types like `MediaWithCrops`, `Link`, `IPublishedContent`) and **`<Name>ViewComponent`** (a `sealed class : ViewComponent`) live in the same `.cs`.

Canonical example: [Aesys.Core/Components/HomePage/HeroBanner/HeroBannerViewComponent.cs](../../../Aesys.Core/Components/HomePage/HeroBanner/HeroBannerViewComponent.cs).

```csharp
public sealed record HeroBannerViewModel(
    string Title,
    string Text,
    MediaWithCrops Background,
    IEnumerable<Link> Buttons
);

public sealed class HeroBannerViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Models.HeroBanner source)
    {
        var vm = new HeroBannerViewModel(
            Title: source.Title,
            Text: source.Text,
            Background: source.Background,
            Buttons: source.Buttons
        );
        return View(vm);
    }
}
```

Rules:

- Map model → ViewModel **inside `Invoke`**. Never pass the raw generated model to the Razor view.
- ViewModel is a `record` (positional, immutable). The view binds against it.
- The ViewComponent only orchestrates: pull values off `source`, project into the VM, return `View(vm)`.

## Invoke signature: interface (Compositions) vs model (Components/Shared) vs primitives (Pure UI)

- **Compositions** (`Aesys.Core/Compositions/<Name>/`) — `Invoke` takes the generated **interface** (`IHeader`, `IFooter`). This lets any page that composes the mixin pass itself in. See [HeaderViewComponent.cs](../../../Aesys.Core/Compositions/Header/HeaderViewComponent.cs). **Caveat — the interface is generated lazily:** ModelsBuilder only emits `I<Name>` once another doctype actually composes the mixin. A brand-new composition with no consumer yet has **no** `I<Name>`, so `Invoke(I<Name> source)` fails to compile (`CS0246`). Until a consumer exists, either take the concrete `Models.<Name>` temporarily, or author the consumer first and regenerate — then switch to the interface. (Consumers that are element blocks force the composition to `IsElement=true`; see [usync-author](../usync-author/SKILL.md) `## IsElement and compositions`.)
- **Page-scoped Components** (`Aesys.Core/Components/<Page>/<Name>/`, `IsElement=true`) — `Invoke` takes the generated **class** (`Models.HeroBanner`). Elements aren't shared; the concrete type is fine.
- **Shared blocks** (`Aesys.Core/Shared/<Name>/`, `IsElement=true`) — same as page-scoped Components: `Invoke` takes the generated class via `Models.<Name>`.
- **Pure UI** (`Aesys.Core/Components/UI/<Name>/`) — `Invoke` takes **plain primitives** passed by the caller (e.g. `Invoke(string label, string href = "", string variant = "primary", string size = "md")`). No `source` parameter, no `Models.X` reference, no generated model exists. See [ButtonViewComponent.cs](../../../Aesys.Core/Components/UI/Button/ButtonViewComponent.cs) for the canonical example.
- **Pages** (`Aesys.Core/Pages/<Name>/`) typically render via their template (`Views/<Alias>/<Alias>.cshtml`), not a ViewComponent. Skip the VC unless a page needs to be embedded somewhere as a fragment.

## Namespace-vs-class collision

Folders like `Aesys.Core/Components/HomePage/HeroBanner/` make the C# namespace `Aesys.Core.Components.HomePage.HeroBanner`. The ModelsBuilder-generated class is `Aesys.Core.Models.HeroBanner`. Inside the folder's namespace, the unqualified name `HeroBanner` resolves to the **namespace**, not the class — the namespace shadows it.

Workaround: reference the model with the `Models.` prefix:

```csharp
public IViewComponentResult Invoke(Models.HeroBanner source) { ... }
```

`Models.` is the alias picked up from `using Aesys.Core.Models;` at the top of the file. Don't fully qualify (`Aesys.Core.Models.HeroBanner`) — `Models.X` is the established convention.

## Where the Razor partial lives

ASP.NET Core ViewComponent discovery looks under `Views/Shared/Components/<ComponentName>/Default.cshtml`. The view is **NOT** co-located with the `.cs` source — it lives under `Aesys.Web/`:

```
Aesys.Web/Views/Shared/Components/
├── Header/Default.cshtml
├── Footer/Default.cshtml
└── HeroBanner/Default.cshtml
```

The `<ComponentName>` folder matches the class name minus the `ViewComponent` suffix (`HeroBannerViewComponent` → `HeroBanner/`). The view inherits the ViewModel:

```cshtml
@model Aesys.Core.Components.HomePage.HeroBanner.HeroBannerViewModel
```

`Aesys.Web/Views/_ViewImports.cshtml` already imports the relevant namespaces — confirm before adding fully-qualified `@model` references.

## Custom template discovery (pages)

A page template can live at either `Views/<Alias>.cshtml` (default) or `Views/<Alias>/<Alias>.cshtml` (preferred — keeps related view files together). The second location is enabled by [Aesys.Web/ViewLocations/DoctypeFolderViewLocationExpander.cs](../../../Aesys.Web/ViewLocations/DoctypeFolderViewLocationExpander.cs), registered in `Program.cs`. Example: [Aesys.Web/Views/HomePage/HomePage.cshtml](../../../Aesys.Web/Views/HomePage/HomePage.cshtml).

## Block dispatch pattern

When a page renders a Block List property, the page template iterates blocks and invokes the matching ViewComponent by `ContentType.Alias`:

```cshtml
@foreach (var block in Model.Components)
{
    @await Component.InvokeAsync(block.Content.ContentType.Alias, new { source = block.Content })
}
```

See [Aesys.Web/Views/HomePage/HomePage.cshtml](../../../Aesys.Web/Views/HomePage/HomePage.cshtml) for the live example. Each block's element type has a registered ViewComponent whose name matches its alias (e.g. alias `heroBanner` → `HeroBannerViewComponent`).

For richer dispatch (Block List/Grid/single) via partials in `Aesys.Web/Views/Partials/`, see the [umbraco-blocks](../umbraco-blocks/SKILL.md) skill.

## When to skip the ViewComponent

Not every doctype needs one:

- **Pages** rendered via their template (`Views/<Alias>/<Alias>.cshtml`) — no VC needed unless the page is also embedded as a fragment elsewhere.
- **Settings-only compositions** (e.g. `GlobalSettings`) that hold properties but have no render output of their own — the consumer reads `source.SomeProperty` directly in its template/VM.
- **DataTypes, MediaTypes, MemberTypes, Templates** — these are not DocumentTypes; no render layer applies.
- **Wrappers that need to accept Razor children** — a ViewComponent **cannot** take a Razor child body (`Component.InvokeAsync` has no child slot). Use a **Tag Helper** instead. The canonical case is section chrome: see below.

## Section chrome — use the `<section-block>` tag helper, not a ViewComponent

Section chrome (the outer `<section>` + optional full-bleed background image + scrim, or a plain light section + gray bottom border, plus the inner `wrapper` container) is rendered by the [SectionTagHelper](../../../Aesys.Web/TagHelpers/SectionTagHelper.cs), **not** a ViewComponent — because only a tag helper can wrap arbitrary Razor children.

In a block/page partial, wrap the content:

```cshtml
<section-block bg-image="@(Model.Background?.Url())" data-component="hero-banner">
    @* ...arbitrary Razor children, other Component.InvokeAsync calls, etc... *@
</section-block>
```

- `bg-image` set → dark full-bleed surface + image + scrim. Empty → plain light section + gray bottom border (`SectionVariants.WithImage` vs `Plain`).
- `class` is TwMerge'd onto the chrome; any other attribute (`id`, `data-component`, `aria-*`) passes through. `data-component` defaults to `"section"` only if you don't set one.
- The tag helper owns the **first-block header offset** (reads/clears `HttpContext.Items["IsFirstComponent"]`), so don't also call `Html.HeaderOffset()` on a `<section-block>`.
- The `background` field itself comes from the `Section` composition mixin — a block gains it by composing `section`. Canonical consumers: [HeroBanner](../../../Aesys.Web/Views/Shared/Components/HeroBanner/Default.cshtml), [TextWithImage](../../../Aesys.Web/Views/Shared/Components/TextWithImage/Default.cshtml).

## When to invoke this skill

- User asks to add or modify the render layer for a doctype.
- User is editing or creating a `*ViewComponent.cs` file under `Aesys.Core/`.
- User is editing a `Default.cshtml` under `Aesys.Web/Views/Shared/Components/`.
- User asks why a generated class is shadowed by a namespace (`Models.X` workaround question).
- User is wiring up block dispatch on a page template.

## When NOT to invoke this skill

- Authoring the doctype `.config` itself → [usync-author](../usync-author/SKILL.md).
- Choosing which DataType a property should use → [umbraco-datatypes](../umbraco-datatypes/SKILL.md).
- Designing a Block List/Grid composition (which blocks belong where) → [umbraco-blocks](../umbraco-blocks/SKILL.md).
- Editing ModelsBuilder-generated files under `Aesys.Core/Generated/` — off-limits per [CLAUDE.md](../../../CLAUDE.md) `## Umbraco specifics`.
