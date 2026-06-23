import { defineComponent } from '@/lib/component';

// Contact form submit + swap.
//
// The form posts to the ContactForm surface controller. We intercept the submit,
// send it via fetch, and replace the contents of the stable [data-contact-form]
// wrapper with the server's response:
//   - 422 -> the re-rendered form partial WITH validation messages (we keep the
//            visitor's input because the server echoes the posted model back).
//   - 200 -> the success partial; the form is gone, the thank-you shows.
//
// We bind ONE delegated submit listener to the stable wrapper (not the form), so
// the form can be freely replaced on each 422 without re-hooking — and a
// backoffice DOM swap that recreates it keeps working too.
//
// Without JS the same POST works as a full-page navigation (the surface controller
// returns the partial), so this is progressive enhancement, not a hard dependency.
defineComponent<HTMLElement>('[data-component="contact-form"]', (root) => {
  const wrapper = root.querySelector<HTMLElement>('[data-contact-form]');
  if (!wrapper) return;

  // Mark fields whose validation span got a message, so the input shows its
  // invalid ring (CSS keys off aria-invalid). Runs after every swap-in.
  function reflectFieldErrors(form: HTMLFormElement) {
    form
      .querySelectorAll<HTMLElement>('[data-valmsg-for], .field-validation-error')
      .forEach((span) => {
        const name = span.getAttribute('data-valmsg-for');
        if (!name) return;
        const field = form.querySelector<HTMLElement>(`[name="${CSS.escape(name)}"]`);
        if (!field) return;
        const hasError = span.textContent != null && span.textContent.trim().length > 0;
        if (hasError) field.setAttribute('aria-invalid', 'true');
        else field.removeAttribute('aria-invalid');
      });
  }

  async function submit(form: HTMLFormElement) {
    const button = form.querySelector<HTMLButtonElement>('[data-submit]');
    const label = form.querySelector<HTMLElement>('[data-submit-label]');
    const originalLabel = label?.textContent ?? '';
    // The localized "Sending…" text is rendered server-side onto the button so the
    // client doesn't hardcode a language (see _Form.cshtml data-submitting).
    const submittingLabel = button?.getAttribute('data-submitting') ?? '';
    if (button) button.disabled = true;
    if (label) label.textContent = submittingLabel;

    try {
      const res = await fetch(form.action, {
        method: 'POST',
        headers: { 'X-Requested-With': 'fetch' },
        body: new FormData(form),
      });

      // 422 (validation/mail error) and 200 (success) both carry HTML for the
      // wrapper. Any other status is unexpected — restore the button and bail.
      if (!res.ok && res.status !== 422) {
        throw new Error(`Submit failed: ${res.status}`);
      }

      const html = (await res.text()).trim();
      const fragment = document.createRange().createContextualFragment(html);
      wrapper!.replaceChildren(fragment);

      const newForm = wrapper!.querySelector<HTMLFormElement>('[data-contact-form-el]');
      if (newForm) {
        reflectFieldErrors(newForm);
        // Move focus to the first invalid field (or the summary) for keyboard /
        // screen-reader users.
        const firstInvalid = newForm.querySelector<HTMLElement>('[aria-invalid="true"]');
        firstInvalid?.focus();
      }
    } catch (err) {
      console.error(err);
      if (button) button.disabled = false;
      if (label) label.textContent = originalLabel;
    }
  }

  wrapper.addEventListener('submit', (e) => {
    const form = (e.target as Element).closest<HTMLFormElement>('[data-contact-form-el]');
    if (!form) return;
    e.preventDefault();
    void submit(form);
  });
});
