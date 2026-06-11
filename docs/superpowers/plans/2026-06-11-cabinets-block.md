# Cabinets Block Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a shared `Cabinets` block to standard content pages — a section with intro/background compositions and a 2-column grid of cabinet cards (image, title, description, "Learn More" link).

**Architecture:** Two new `IsElement=true` ContentTypes (`Cabinets` + `CabinetItem`) under `Aesys.Core/Shared/Cabinets/`, a new `BLCabinetItems` DataType, a `CabinetsViewComponent`, and a Razor template. The block is registered in `BLStandardPage` so it appears in standard content pages' `components` block list. Follows the exact same pattern as `TextWithImage`.

**Tech Stack:** Umbraco 17, uSync code-first `.config`, C# ViewComponent + ViewModel record, Razor `.cshtml`, Tailwind v4, `<section-block>` tag helper, `SectionVariants`.

---

## File Map

| Action | File |
|---|---|
| Create | `Aesys.Core/Shared/Cabinets/CabinetItem/cabinetitem.config` |
| Create | `Aesys.Core/Shared/Cabinets/cabinets.config` |
| Create | `Aesys.Web/uSync/v17/DataTypes/BLCabinetItems.config` |
| Create | `Aesys.Core/Shared/Cabinets/CabinetsViewComponent.cs` |
| Create | `Aesys.Web/Views/Shared/Components/Cabinets/Default.cshtml` |
| Modify | `Aesys.Web/uSync/v17/DataTypes/BLStandardPage.config` |

---

## Task 1: CabinetItem ContentType config

**Files:**
- Create: `Aesys.Core/Shared/Cabinets/CabinetItem/cabinetitem.config`

- [ ] **Step 1: Create the folder and config file**

Create `Aesys.Core/Shared/Cabinets/CabinetItem/cabinetitem.config` with this exact content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<ContentType Key="07fa5f54-60a3-49ab-a8ba-5bf3c07d11f7" Alias="cabinetItem" Level="4">
  <Info>
    <Name>Cabinet Item</Name>
    <Icon>icon-picture color-green</Icon>
    <Thumbnail>folder.png</Thumbnail>
    <Description><![CDATA[A single cabinet card: image, title, description and a learn-more link. Used inside the Cabinets block.]]></Description>
    <AllowAtRoot>False</AllowAtRoot>
    <ListView>00000000-0000-0000-0000-000000000000</ListView>
    <Variations>Nothing</Variations>
    <IsElement>true</IsElement>
    <HistoryCleanup>
      <PreventCleanup>False</PreventCleanup>
      <KeepAllVersionsNewerThanDays></KeepAllVersionsNewerThanDays>
      <KeepLatestVersionPerDayForDays></KeepLatestVersionPerDayForDays>
    </HistoryCleanup>
    <Folder>Shared</Folder>
    <Compositions />
    <DefaultTemplate></DefaultTemplate>
    <AllowedTemplates />
  </Info>
  <Structure />
  <GenericProperties>
    <GenericProperty>
      <Key>6c63d803-6b6a-40c7-bf5b-8fa086e98911</Key>
      <Name>Image</Name>
      <Alias>image</Alias>
      <Definition>ad9f0cf2-bda2-45d5-9ea1-a63cfc873fd3</Definition>
      <Type>Umbraco.MediaPicker3</Type>
      <Mandatory>false</Mandatory>
      <Validation></Validation>
      <Description><![CDATA[Cabinet photo. Displayed at full card width, natural height.]]></Description>
      <SortOrder>0</SortOrder>
      <Tab Alias="content">Content</Tab>
      <Variations>Nothing</Variations>
      <MandatoryMessage></MandatoryMessage>
      <ValidationRegExpMessage></ValidationRegExpMessage>
      <LabelOnTop>false</LabelOnTop>
    </GenericProperty>
    <GenericProperty>
      <Key>c4315018-bbfa-4978-955a-5b46e3bc81ef</Key>
      <Name>Title</Name>
      <Alias>title</Alias>
      <Definition>0cc0eba1-9960-42c9-bf9b-60e150b429ae</Definition>
      <Type>Umbraco.TextBox</Type>
      <Mandatory>false</Mandatory>
      <Validation></Validation>
      <Description><![CDATA[Cabinet name, rendered as a heading.]]></Description>
      <SortOrder>1</SortOrder>
      <Tab Alias="content">Content</Tab>
      <Variations>Nothing</Variations>
      <MandatoryMessage></MandatoryMessage>
      <ValidationRegExpMessage></ValidationRegExpMessage>
      <LabelOnTop>false</LabelOnTop>
    </GenericProperty>
    <GenericProperty>
      <Key>6d4d4737-e908-4461-8064-6f34790ae553</Key>
      <Name>Description</Name>
      <Alias>description</Alias>
      <Definition>c6bac0dd-4ab9-45b1-8e30-e4b619ee5da3</Definition>
      <Type>Umbraco.TextArea</Type>
      <Mandatory>false</Mandatory>
      <Validation></Validation>
      <Description><![CDATA[Short cabinet description shown below the title.]]></Description>
      <SortOrder>2</SortOrder>
      <Tab Alias="content">Content</Tab>
      <Variations>Nothing</Variations>
      <MandatoryMessage></MandatoryMessage>
      <ValidationRegExpMessage></ValidationRegExpMessage>
      <LabelOnTop>false</LabelOnTop>
    </GenericProperty>
    <GenericProperty>
      <Key>2878dce2-2b39-48d8-a082-cb27b9bfdf89</Key>
      <Name>Link</Name>
      <Alias>link</Alias>
      <Definition>a1eb33f6-5c81-46a1-adb0-e2ba272aa350</Definition>
      <Type>Umbraco.MultiUrlPicker</Type>
      <Mandatory>false</Mandatory>
      <Validation></Validation>
      <Description><![CDATA[The "Learn More" destination for this cabinet.]]></Description>
      <SortOrder>3</SortOrder>
      <Tab Alias="content">Content</Tab>
      <Variations>Nothing</Variations>
      <MandatoryMessage></MandatoryMessage>
      <ValidationRegExpMessage></ValidationRegExpMessage>
      <LabelOnTop>false</LabelOnTop>
    </GenericProperty>
  </GenericProperties>
  <Tabs>
    <Tab>
      <Key>4592db56-a533-4410-b08e-254e4e6fe77a</Key>
      <Caption>Content</Caption>
      <Alias>content</Alias>
      <Type>Tab</Type>
      <SortOrder>0</SortOrder>
    </Tab>
  </Tabs>
</ContentType>
```

- [ ] **Step 2: Commit**

```bash
git add Aesys.Core/Shared/Cabinets/CabinetItem/cabinetitem.config
git commit -m "feat: add CabinetItem ContentType config"
```

---

## Task 2: BLCabinetItems DataType

**Files:**
- Create: `Aesys.Web/uSync/v17/DataTypes/BLCabinetItems.config`

- [ ] **Step 1: Create the DataType config**

Create `Aesys.Web/uSync/v17/DataTypes/BLCabinetItems.config` with this exact content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<DataType Key="86462131-5ee2-4a81-a84b-9b925d3c2349" Alias="BL - Cabinet Items" Level="2">
  <Info>
    <Name>BL - Cabinet Items</Name>
    <EditorAlias>Umbraco.BlockList</EditorAlias>
    <EditorUIAlias>Umb.PropertyEditorUi.BlockList</EditorUIAlias>
    <Folder>BL</Folder>
  </Info>
  <Config><![CDATA[{
  "blocks": [
    {
      "contentElementTypeKey": "07fa5f54-60a3-49ab-a8ba-5bf3c07d11f7",
      "label": "Cabinet - {=title}"
    }
  ]
}]]></Config>
</DataType>
```

- [ ] **Step 2: Commit**

```bash
git add Aesys.Web/uSync/v17/DataTypes/BLCabinetItems.config
git commit -m "feat: add BL - Cabinet Items DataType"
```

---

## Task 3: Cabinets ContentType config

**Files:**
- Create: `Aesys.Core/Shared/Cabinets/cabinets.config`

- [ ] **Step 1: Create the config file**

Create `Aesys.Core/Shared/Cabinets/cabinets.config` with this exact content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<ContentType Key="d4018f81-db98-4e52-94d9-b5db13dc632e" Alias="cabinets" Level="3">
  <Info>
    <Name>Cabinets</Name>
    <Icon>icon-grid color-green</Icon>
    <Thumbnail>folder.png</Thumbnail>
    <Description><![CDATA[Shared block: an Intro Text (eyebrow/title/text/button) above a 2-column grid of cabinet cards. Each card has an image, title, description and a learn-more link. Optional section background via the Section mixin.]]></Description>
    <AllowAtRoot>False</AllowAtRoot>
    <ListView>00000000-0000-0000-0000-000000000000</ListView>
    <Variations>Nothing</Variations>
    <IsElement>true</IsElement>
    <HistoryCleanup>
      <PreventCleanup>False</PreventCleanup>
      <KeepAllVersionsNewerThanDays></KeepAllVersionsNewerThanDays>
      <KeepLatestVersionPerDayForDays></KeepLatestVersionPerDayForDays>
    </HistoryCleanup>
    <Folder>Shared</Folder>
    <Compositions>
      <Composition Key="76fed7ca-df43-4720-a52e-8659c9549fb1">introText</Composition>
      <Composition Key="02956852-8784-4059-b38f-a21bb7373a44">section</Composition>
    </Compositions>
    <DefaultTemplate></DefaultTemplate>
    <AllowedTemplates />
  </Info>
  <Structure />
  <GenericProperties>
    <GenericProperty>
      <Key>1ee5c167-38cc-47e9-8dc2-fd9010d49da3</Key>
      <Name>Items</Name>
      <Alias>items</Alias>
      <Definition>86462131-5ee2-4a81-a84b-9b925d3c2349</Definition>
      <Type>Umbraco.BlockList</Type>
      <Mandatory>false</Mandatory>
      <Validation></Validation>
      <Description><![CDATA[The cabinet cards. Each has an image, title, description and learn-more link.]]></Description>
      <SortOrder>5</SortOrder>
      <Tab Alias="content">Content</Tab>
      <Variations>Nothing</Variations>
      <MandatoryMessage></MandatoryMessage>
      <ValidationRegExpMessage></ValidationRegExpMessage>
      <LabelOnTop>false</LabelOnTop>
    </GenericProperty>
  </GenericProperties>
  <Tabs>
    <Tab>
      <Key>cefb0a9b-2720-4766-9628-e6f1af5113b7</Key>
      <Caption>Content</Caption>
      <Alias>content</Alias>
      <Type>Tab</Type>
      <SortOrder>0</SortOrder>
    </Tab>
  </Tabs>
</ContentType>
```

- [ ] **Step 2: Commit**

```bash
git add Aesys.Core/Shared/Cabinets/cabinets.config
git commit -m "feat: add Cabinets ContentType config"
```

---

## Task 4: Register Cabinets in BLStandardPage and bundle

**Files:**
- Modify: `Aesys.Web/uSync/v17/DataTypes/BLStandardPage.config`

- [ ] **Step 1: Add Cabinets to the allowed blocks**

In `Aesys.Web/uSync/v17/DataTypes/BLStandardPage.config`, add the Cabinets block entry to the `"blocks"` array. The file currently contains 4 blocks. Add a 5th:

```xml
<?xml version="1.0" encoding="utf-8"?>
<DataType Key="5daf31e2-fb39-4a1f-a147-77555144641d" Alias="BL - Standard Page" Level="2">
  <Info>
    <Name>BL - Standard Page</Name>
    <EditorAlias>Umbraco.BlockList</EditorAlias>
    <EditorUIAlias>Umb.PropertyEditorUi.BlockList</EditorUIAlias>
    <Folder>BL</Folder>
  </Info>
  <Config><![CDATA[{
  "blocks": [
    {
      "contentElementTypeKey": "b0c51460-6d98-44e8-8b81-22c4d2839082",
      "label": "Text With Image - {=title}"
    },
    {
      "contentElementTypeKey": "0898df7b-a895-4363-8565-d247fa7db80d",
      "label": "Cards - {=title}"
    },
    {
      "contentElementTypeKey": "bfc7ae19-ff25-49a3-9dcb-f402e80f177b",
      "label": "Solutions - {=title}"
    },
    {
      "contentElementTypeKey": "c0a7f3e1-2b4d-4f6a-8c1e-9d3b5a7e1f20",
      "label": "Contact Form - {=title}"
    },
    {
      "contentElementTypeKey": "d4018f81-db98-4e52-94d9-b5db13dc632e",
      "label": "Cabinets - {=title}"
    }
  ]
}]]></Config>
</DataType>
```

- [ ] **Step 2: Bundle configs**

```bash
mise run usync:bundle
```

Expected: output says files copied into `Aesys.Web/uSync/v17/ContentTypes/` — you should see `cabinets.config` and `cabinetitem.config` listed.

- [ ] **Step 3: Commit**

```bash
git add Aesys.Web/uSync/v17/DataTypes/BLStandardPage.config Aesys.Web/uSync/v17/ContentTypes/
git commit -m "feat: register Cabinets in BLStandardPage and bundle ContentTypes"
```

---

## Task 5: CabinetsViewComponent

**Files:**
- Create: `Aesys.Core/Shared/Cabinets/CabinetsViewComponent.cs`

- [ ] **Step 1: Create the ViewComponent**

Create `Aesys.Core/Shared/Cabinets/CabinetsViewComponent.cs`:

```csharp
using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;

namespace Aesys.Core.Shared.Cabinets;

public sealed record CabinetItemViewModel(
    MediaWithCrops Image,
    string Title,
    string Description,
    Link Link
);

public sealed record CabinetsViewModel(
    IIntroText Intro,
    MediaWithCrops Background,
    string BackgroundColor,
    IReadOnlyList<CabinetItemViewModel> Items
);

public sealed class CabinetsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Models.Cabinets source)
    {
        var items =
            source
                .Items?.Select(b => b.Content)
                .OfType<Models.CabinetItem>()
                .Select(c => new CabinetItemViewModel(
                    c.Image,
                    c.Title,
                    c.Description,
                    c.Link
                ))
                .ToList()
            ?? [];

        var vm = new CabinetsViewModel(
            Intro: source,
            Background: source.Background,
            BackgroundColor: source.BackgroundColor,
            Items: items
        );

        return View(vm);
    }
}
```

> **Note:** `Models.Cabinets` and `Models.CabinetItem` are ModelsBuilder-generated. They will not exist until the app has booted once with the new ContentTypes imported via uSync (Task 4). If the project fails to build at this step with `CS0234`/`CS0246`, that means the app hasn't been run yet to regenerate models. Boot the app (`mise run dev`), wait for uSync import + ModelsBuilder regen, then come back to compile this file.

- [ ] **Step 2: Commit**

```bash
git add Aesys.Core/Shared/Cabinets/CabinetsViewComponent.cs
git commit -m "feat: add CabinetsViewComponent and ViewModels"
```

---

## Task 6: Razor template

**Files:**
- Create: `Aesys.Web/Views/Shared/Components/Cabinets/Default.cshtml`

- [ ] **Step 1: Create the directory and template**

Create `Aesys.Web/Views/Shared/Components/Cabinets/Default.cshtml`:

```cshtml
@model Aesys.Core.Shared.Cabinets.CabinetsViewModel
@using Aesys.Core.Components.UI.Section

@{
    var theme = SectionVariants.Theme(Model.BackgroundColor, Model.Background is not null);
}

<section-block bg-image="@(Model.Background?.Url())" bg-color="@Model.BackgroundColor" data-component="cabinets">
    <div class="flex flex-col gap-12 md:gap-16">
        @await Component.InvokeAsync("IntroText", new { source = Model.Intro, theme })

        @if (Model.Items.Count > 0)
        {
            <ul class="grid grid-cols-1 gap-8 sm:grid-cols-2">
                @foreach (var item in Model.Items)
                {
                    <li class="@Html.Cn("flex flex-col overflow-hidden rounded-xl", theme == "dark" ? "bg-aesys-800/60 text-white" : "bg-surface-light")">
                        @if (item.Image is not null)
                        {
                            <img src="@item.Image.Url()" alt="" class="h-auto w-full object-cover" />
                        }

                        <div class="flex flex-1 flex-col gap-3 p-6">
                            @if (!string.IsNullOrWhiteSpace(item.Title))
                            {
                                <h3 class="text-h4 font-bold">@item.Title</h3>
                            }

                            @if (!string.IsNullOrWhiteSpace(item.Description))
                            {
                                <p class="whitespace-pre-line text-base">@item.Description</p>
                            }

                            @if (item.Link is not null)
                            {
                                <div class="mt-auto flex justify-end pt-2">
                                    <a href="@item.Link.Url" target="@item.Link.Target" class="@Html.Cn("inline-flex items-center gap-1 text-sm font-semibold underline-offset-2 hover:underline", theme == "dark" ? "text-white" : "text-aesys-800")">
                                        @(string.IsNullOrWhiteSpace(item.Link.Name) ? "Learn More" : item.Link.Name)
                                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" fill="currentColor" class="size-4" aria-hidden="true">
                                            <path fill-rule="evenodd" d="M6.22 4.22a.75.75 0 0 1 1.06 0l3.25 3.25a.75.75 0 0 1 0 1.06l-3.25 3.25a.75.75 0 0 1-1.06-1.06L8.94 8 6.22 5.28a.75.75 0 0 1 0-1.06Z" clip-rule="evenodd" />
                                        </svg>
                                    </a>
                                </div>
                            }
                        </div>
                    </li>
                }
            </ul>
        }
    </div>
</section-block>
```

- [ ] **Step 2: Commit**

```bash
git add Aesys.Web/Views/Shared/Components/Cabinets/Default.cshtml
git commit -m "feat: add Cabinets Razor template"
```

---

## Task 7: Boot app and verify

- [ ] **Step 1: Start the dev stack**

```bash
mise run dev
```

Watch the startup log for:
1. `uSync: Importing ContentTypes` — should mention `cabinets` and `cabinetItem`
2. `uSync: Importing DataTypes` — should mention `BL - Cabinet Items`
3. ModelsBuilder output — `Aesys.Core/Generated/` should gain `Cabinets.generated.cs` and `CabinetItem.generated.cs`

If `models.err` appears in `Aesys.Core/Generated/`, open it — a common cause is a composition `IsElement` mismatch (the `introText` and `section` compositions are already `IsElement=true` so this should be fine).

- [ ] **Step 2: Verify in backoffice**

Open `http://localhost:<port>/umbraco` and log in as `admin@local` / `LocalDev1234!`.

Navigate to **Settings → Document Types**. Confirm:
- `Cabinets` exists under the `Shared` folder, with compositions `introText` + `section` and an `items` BlockList property
- `Cabinet Item` exists under the `Shared` folder, with `image`, `title`, `description`, `link` properties

Navigate to **Settings → Data Types → BL**. Confirm:
- `BL - Cabinet Items` exists and lists Cabinet Item as its only allowed block
- `BL - Standard Page` lists Cabinets as one of its allowed blocks

- [ ] **Step 3: Add a Cabinets block to a standard content page**

In the backoffice content tree, open any standard content page. Add a **Cabinets** block to the `components` block list. Add 2 Cabinet Item cards (upload a test image, fill in title, description, link). Save and preview the page.

Expected: a section renders with the intro text (if filled) and a 2-column card grid. Each card shows the image at the top, then title, description, and a "Learn More →" arrow link at the bottom right. On dark background, cards use `bg-aesys-800/60 text-white`; on light background, `bg-surface-light`.

- [ ] **Step 4: Commit any generated model files**

```bash
git add Aesys.Core/Generated/Cabinets.generated.cs Aesys.Core/Generated/CabinetItem.generated.cs
git commit -m "chore: add ModelsBuilder-generated files for Cabinets and CabinetItem"
```
