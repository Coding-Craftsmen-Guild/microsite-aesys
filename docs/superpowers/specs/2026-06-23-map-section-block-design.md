# Map Section Block — Design

**Date:** 2026-06-23
**Status:** Approved

## Problem

The site has a "standard section" left column (eyebrow / title / text / button via
the `introText` mixin) reused across blocks, and `TextWithImage` pairs that column
with an optional image in a 50/50 row. There is no block that pairs the standard
section text with an **embedded Google Map**.

Concrete need: a location/contact section — standard section text on the left, a
Google Maps embed (with rounded edges) on the right.

## Goal

Add a new **Map** block: everything from the standard section (background, eyebrow,
title, text, button) on the left, plus a Google Maps `<iframe>` embed on the right
in a rounded container. The map side is editor-flippable (Left/Right). It is a
near-clone of `TextWithImage` with the image replaced by a map embed string.

## Bucket

**Shared block** (`IsElement=true`), folder `Aesys.Core/Shared/Map/` — same bucket
as [TextWithImage](../../../Aesys.Core/Shared/TextWithImage/) (reusable across pages,
not page-specific). Razor partial at
`Aesys.Web/Views/Shared/Components/Map/Default.cshtml` (ViewComponent discovery is by
class name, not source folder).

## Changes

### 1. Schema — `Aesys.Core/Shared/Map/map.config`

DocumentType `map`, `IsElement=true`, `Folder=Shared`. Composes the same two mixins
`TextWithImage` uses:

- `introText` (Key `76fed7ca-df43-4720-a52e-8659c9549fb1`) — eyebrow / title / text / button.
- `section` (Key `02956852-8784-4059-b38f-a21bb7373a44`) — `background` image + `backgroundColor`.

Own properties (both on a **Content** tab, matching TextWithImage's layout):

| Name | Alias | DataType | Definition GUID | Editor | Notes |
|---|---|---|---|---|---|
| Map Embed | `mapEmbed` | Textarea | `c6bac0dd-4ab9-45b1-8e30-e4b619ee5da3` | `Umbraco.TextArea` | Editor pastes Google Maps' "Embed a map" full `<iframe>` HTML. Multi-line → Textarea, not Textstring. |
| Map Position | `mapPosition` | DropDown.Flexible | `6f248eb5-42fd-4199-a51b-5107b93b97d2` | `Umbraco.DropDown.Flexible` | **Reuses** the exact DataType TextWithImage's `imagePosition` uses (Left / Right, Right default). |

New GUIDs to assign (must be verified globally unique across `Aesys.Core/` and
`Aesys.Web/uSync/` per the usync-author GUID rule before writing):

- ContentType `Key`
- `mapEmbed` property `Key`
- `mapPosition` property `Key`
- Content `Tab` `Key`

`Variations` = `Nothing` throughout (matches TextWithImage). `AllowAtRoot=False`.

### 2. ViewComponent — `Aesys.Core/Shared/Map/MapViewComponent.cs`

Mirrors [TextWithImageViewComponent](../../../Aesys.Core/Shared/TextWithImage/TextWithImageViewComponent.cs):

```csharp
namespace Aesys.Core.Shared.Map;

public sealed record MapViewModel(
    IIntroText Intro,
    MediaWithCrops Background,
    string BackgroundColor,
    string MapEmbed,
    string MapPosition
);

public sealed class MapViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Models.Map source)
    {
        var vm = new MapViewModel(
            Intro: source,
            Background: source.Background,
            BackgroundColor: source.BackgroundColor,
            MapEmbed: source.MapEmbed,
            MapPosition: source.MapPosition
        );
        return View(vm);
    }
}
```

### 3. View — `Aesys.Web/Views/Shared/Components/Map/Default.cshtml`

Structurally identical to
[TextWithImage's view](../../../Aesys.Web/Views/Shared/Components/TextWithImage/Default.cshtml).
Differences:

- `var hasMap = !string.IsNullOrWhiteSpace(Model.MapEmbed);` replaces `hasImage`.
- Row reversal driven by `mapPosition == "Left"`.
- Right column when `hasMap`: a wrapper `div` carrying `overflow-hidden rounded-xl`
  with `@Html.Raw(Model.MapEmbed)` inside. The raw iframe is output **verbatim**
  (per the safety decision — editors are trusted). The **rounded edges** come from
  the `overflow-hidden rounded-xl` wrapper, not from touching the pasted HTML.
- When `mapEmbed` is empty, the intro spans full width (graceful fallback, like
  TextWithImage's no-image case).

```html
@model Aesys.Core.Shared.Map.MapViewModel
@using Aesys.Core.Components.UI.Section

@{
    var hasMap = !string.IsNullOrWhiteSpace(Model.MapEmbed);
    var theme = SectionVariants.Theme(Model.BackgroundColor, Model.Background is not null);
    var introClass = hasMap ? "flex-1 basis-0 min-w-[20rem]" : "w-full";
    var rowDirection = Model.MapPosition == "Left" ? "md:flex-row-reverse" : "";
}

<section-block bg-image="@(Model.Background?.Url())" bg-color="@Model.BackgroundColor" data-component="map">
    <div class="@Html.Cn("flex flex-wrap items-center gap-12 md:gap-25", rowDirection)">
        @await Component.InvokeAsync("IntroText", new
        {
            source = Model.Intro,
            theme,
            className = introClass,
        })

        @if (hasMap)
        {
            <div class="map-embed flex-1 basis-0 min-w-[20rem] overflow-hidden rounded-xl">
                @Html.Raw(Model.MapEmbed)
            </div>
        }
    </div>
</section-block>
```

### 4. Co-located styles — `Aesys.Web/Views/Shared/Components/Map/map.scss`

A pasted Google Maps iframe carries fixed `width="600" height="450"` and square
corners. The wrapper gives rounded corners (`overflow-hidden`); this reset forces
the embedded iframe to fill the wrapper responsively regardless of the pasted
attributes:

```scss
.map-embed {
  iframe {
    display: block;
    width: 100%;
    min-height: 24rem; // ~384px; sensible map height on desktop and mobile
    border: 0;
  }
}
```

Vite picks this up automatically via the `import.meta.glob('../Views/**/*.{ts,scss}')`
in `main.ts` — no registration. No `.ts` is needed (no behavior).

### 5. Availability — `Aesys.Web/uSync/v17/DataTypes/BLStandardPage.config`

Add the Map ContentType to the `BL - Standard Page` block list `blocks` array only
(not Home Page):

```json
{
  "contentElementTypeKey": "<map ContentType Key>",
  "label": "Map - {=title}"
}
```

`BLStandardPage.config` Key `5daf31e2-fb39-4a1f-a147-77555144641d`. DataTypes are
backoffice-authored and auto-exported by uSync; this hand-edit to the tracked file is
the code-first source of truth and is imported on next dev boot.

## Bootstrap ordering (required)

`ModelsMode=SourceCodeAuto`: `Models.Map`, `source.MapEmbed`, `source.MapPosition`
are generated by the **running app**, not by `dotnet build`. So the ViewComponent
that references them cannot compile until the app has booted once. Strict order:

1. Write `map.config`.
2. `mise run usync:bundle` (flattens sources into `uSync/v17/ContentTypes/`).
3. Edit `BLStandardPage.config` to list the Map block.
4. Boot via `mise run dev` so uSync imports the schema and ModelsBuilder generates
   `Aesys.Core/Generated/Map.generated.cs` (implementing `IIntroText`). Wait for the
   regen; a content-type Save in the backoffice forces it if needed.
5. **Only then** write `MapViewComponent.cs` and `Default.cshtml`; `dotnet watch`
   rebuilds green. The view + scss are runtime/Vite-compiled and don't block the
   build, but sequence them after to keep the boot clean.

`Map.generated.cs` is regenerated by the generator and must not be hand-edited.

## Testing

Manual verification in the running app:

1. Backoffice → a Standard Page → add block → **Map** appears in the block picker.
2. Add a Map block; fill eyebrow/title/text, paste a Google Maps embed `<iframe>`,
   set Map Position = Right; publish.
3. Front end: standard section text on the left, map on the right with rounded
   corners, responsive width, ~24rem tall.
4. Set Map Position = Left → map and text swap sides on desktop, stack on mobile.
5. Leave Map Embed empty → intro spans full width (no broken empty column).
6. Set a section Background image / Navy color → text theme flips to light (via
   `SectionVariants.Theme`), section chrome renders, map still rounded.

## Out of scope

- No iframe sanitization / Maps-host validation (raw output by decision; editors
  trusted).
- No Home Page availability (Standard Page only).
- No new DataType (Textarea and the existing position dropdown are reused).
- No Google Maps Embed API key / server-side URL construction.
