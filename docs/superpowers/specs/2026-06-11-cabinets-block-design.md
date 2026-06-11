# Cabinets Block — Design Spec

**Date:** 2026-06-11  
**Status:** Approved

## Overview

A new shared block that renders a 2-column grid of cabinet cards on standard content pages. Each card shows an image (top), title, description, and a "Learn More" link. The section supports dark/light theming and an intro text header via the standard compositions.

## Bucket

**Shared block** — `Aesys.Core/Shared/Cabinets/`. Appears on any standard content page's `components` block list. Follows the same pattern as `TextWithImage` and `ContactForm`.

## Element Types

### `Cabinets` (section block)

- `IsElement=true`
- Inherits: `introText`, `section`
- Properties:
  - `items` — BlockList (DataType: `BLCabinetItems`), allows `CabinetItem`

### `CabinetItem` (child element)

- `IsElement=true`
- Located at `Aesys.Core/Shared/Cabinets/CabinetItem/`
- Properties:
  - `image` — `Umbraco.MediaPicker3` (single, no crops)
  - `title` — `Umbraco.TextBox`
  - `description` — `Umbraco.TextArea`
  - `link` — `Umbraco.MultiUrlPicker` (single)

## DataTypes

- **`BLCabinetItems`** — new BlockList DataType, allows only `CabinetItem`. Stored in `Aesys.Web/uSync/v17/DataTypes/BLCabinetItems.config`.
- **`BLStandardPage`** — existing DataType, add `Cabinets` to its allowed blocks.

## ViewComponent

**File:** `Aesys.Core/Shared/Cabinets/CabinetsViewComponent.cs`

```csharp
public sealed record CabinetItemViewModel(MediaWithCrops Image, string Title, string Description, Link Link);

public sealed record CabinetsViewModel(
    IIntroText Intro,             // source itself, implements IIntroText via composition
    MediaWithCrops Background,
    string BackgroundColor,
    IReadOnlyList<CabinetItemViewModel> Items
);

public sealed class CabinetsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Models.Cabinets source) { ... }
}
```

Maps `source.Items` (BlockList) → `IReadOnlyList<CabinetItemViewModel>` via `.Content as Models.CabinetItem`.

## Template

**File:** `Aesys.Web/Views/Shared/Components/Cabinets/Default.cshtml`

Structure:
```
<section-block bg-image="..." bg-color="...">
    IntroText component (theme-aware)
    <ul class="grid grid-cols-1 gap-8 sm:grid-cols-2">
        foreach item:
            <li>
                <img src="..." width 100% height auto rounded-md />
                <div> (padding area)
                    <h3 class="text-h4 font-bold">title</h3>
                    <p class="text-base">description</p>
                    <a href="..." class="...">Learn More →</a>  (bottom-right, theme-aware)
                </div>
            </li>
    </ul>
</section-block>
```

No `CabinetsVariants.cs` — the grid is fixed 2-col and theme classes are simple enough to inline.

## Registration

- Add `Cabinets` to the allowed block types in `Aesys.Web/uSync/v17/DataTypes/BLStandardPage.config`.
- Run `mise run usync:bundle` after authoring configs.

## Files to Create

| File | Purpose |
|---|---|
| `Aesys.Core/Shared/Cabinets/cabinets.config` | Cabinets ContentType |
| `Aesys.Core/Shared/Cabinets/CabinetItem/cabinetitem.config` | CabinetItem ContentType |
| `Aesys.Core/Shared/Cabinets/CabinetsViewComponent.cs` | ViewComponent + ViewModels |
| `Aesys.Web/Views/Shared/Components/Cabinets/Default.cshtml` | Razor template |
| `Aesys.Web/uSync/v17/DataTypes/BLCabinetItems.config` | BlockList DataType for items |

## Files to Modify

| File | Change |
|---|---|
| `Aesys.Web/uSync/v17/DataTypes/BLStandardPage.config` | Add Cabinets to allowed blocks |
