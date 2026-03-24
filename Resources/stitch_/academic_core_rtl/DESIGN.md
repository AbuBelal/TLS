# Design System Specification: The Academic Architect

## 1. Overview & Creative North Star
**Creative North Star: "The Administrative Sanctuary"**
This design system rejects the cluttered, spreadsheet-heavy aesthetic of legacy educational software. Instead, it embraces an "Editorial Administrative" style—combining the authority of a high-end institution with the clarity of a modern workspace. 

By leveraging **RTL-first intentional asymmetry**, we break the rigid grid. Heavy information is balanced by expansive white space (`surface_container_lowest`) and a "Layered Depth" philosophy. The system feels like a series of organized, floating dossiers rather than a flat digital screen.

---

## 2. Colors & Surface Philosophy
The palette transitions from the authoritative `primary` (United Nations Blue) to vibrant, functional accents.

### The "No-Line" Rule
**Prohibit 1px solid borders for sectioning.** 
Structural separation is achieved through background shifts. For example, a student profile card (`surface_container_lowest`) sits atop a dashboard section (`surface_container_low`), which rests on the global `background`. 

### Surface Hierarchy & Nesting
Treat the UI as physical layers of fine paper:
*   **Base:** `background` (#f8f9fb) – The desk surface.
*   **Sectioning:** `surface_container` – Defines large functional areas.
*   **Interaction Hubs:** `surface_container_highest` – Reserved for active sidebars or headers.
*   **Content Cards:** `surface_container_lowest` (#ffffff) – The highest point of focus, appearing "closest" to the user.

### Glass & Gradient Rule
To prevent a "flat" administrative feel, use **Glassmorphism** for floating elements like mobile navigation or dropdowns. 
*   **Token:** `surface` at 80% opacity with a `24px` backdrop blur.
*   **CTAs:** Use a subtle linear gradient (Top-Left to Bottom-Right) from `primary` (#00658e) to `primary_container` (#009edb) to give buttons a "gem-like" tactile quality.

---

## 3. Typography (RTL Optimized)
We utilize **IBM Plex Sans Arabic** for its technical precision and **Tajawal** for editorial headers.

*   **Display (Display-LG/MD):** Used for high-level statistics (e.g., total student count). These should feel like data-art—large, confident, and light-weighted.
*   **Headlines (Headline-SM):** Used for page titles. In RTL, these anchor the top-right, providing an immediate "entry point" for the eye.
*   **Body (Body-MD):** Optimized for long-form reports and student notes. Line height is increased to `1.6` to account for Arabic script ascenders and descenders.
*   **Labels (Label-SM):** Used for metadata (e.g., "Grade 10-B"). Always set in `on_surface_variant` to reduce visual noise.

---

## 4. Elevation & Depth
In this system, depth is a functional tool, not a decoration.

*   **Tonal Layering:** Avoid shadows for static elements. Use `surface_container_low` against `surface_container_lowest` to create a soft, natural lift.
*   **Ambient Shadows:** For "Active" states (e.g., a card being dragged) or modals, use a "Cloud Shadow":
    *   *Y: 12px, Blur: 32px, Color: `on_surface` at 6% opacity.*
*   **The Ghost Border:** If a high-contrast environment requires a border, use `outline_variant` at **15% opacity**. It should be felt, not seen.
*   **RTL Depth:** Shadows should have a slight offset to the *left* (e.g., `-2px`) to acknowledge the right-to-left light source logic of the layout.

---

## 5. Components

### Navigation Sidebar (The Anchor)
*   **Background:** `primary_container` (#009edb).
*   **Active State:** Use a "Pill" shape (`rounded-full`) in `surface_container_lowest` at 15% opacity. Do not use high-contrast boxes.
*   **Typography:** `title-sm`, white text.

### Buttons
*   **Primary:** Gradient of `primary` to `primary_container`. Border radius: `md` (0.375rem).
*   **Secondary:** `surface_container_high` background with `primary` text. No border.
*   **Tertiary:** Ghost style; text only, shifts to `surface_container_low` on hover.

### Content Cards & Lists
*   **Rule:** Forbid divider lines.
*   **Separation:** Use `spacing.6` (1.5rem) of vertical white space or a subtle background toggle (Alternating `surface` and `surface_container_low`).
*   **Interactive List Items:** On hover, scale the item by `1.01` and shift background to `surface_container_highest`.

### Status Indicators (Chips)
*   **Success (Attendance/Pass):** `secondary_container` background with `on_secondary_container` text.
*   **Warning (Late/Pending):** `tertiary_container` background with `on_tertiary_container` text.
*   **Critical (Absent/Fail):** `error_container` background with `on_error_container` text.

---

## 6. Do's and Don'ts

### Do
*   **Do** use asymmetrical margins. Give the right-hand (starting) side of a page title more "breathing room" than the left.
*   **Do** use `primary_fixed_dim` for icons to keep them professional and subdued within dense data tables.
*   **Do** ensure all touch targets are at least `spacing.10` (2.5rem) in height for tablets used by teachers in classrooms.

### Don't
*   **Don't** use pure black (#000000) for text. Always use `on_surface` (#191c1e) to maintain a premium, editorial feel.
*   **Don't** use standard "Drop Shadows" on cards. Stick to Tonal Layering.
*   **Don't** cram data. If a table has more than 8 columns, move secondary data to a "Quick View" drawer using the `surface_container_highest` token.