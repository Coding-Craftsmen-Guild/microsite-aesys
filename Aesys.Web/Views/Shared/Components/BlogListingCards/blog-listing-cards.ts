import { defineComponent } from '@/lib/component';

// Load-more for the Blog Listing Cards grid.
//
// The grid <ul> holds the card <li>s plus a trailing server-rendered button <li>
// ([data-load-more-slot]). The button carries the next request's paging state in
// data-* attributes; the server omits it on the final page.
//
// We bind ONE delegated listener to the stable root (not the button), so the
// button can be freely replaced on each load without re-hooking — and a
// backoffice DOM swap that recreates the button keeps working too. The click
// handler is resolved per-click via closest(), so it always targets whatever
// button is currently in the DOM.
defineComponent<HTMLElement>('[data-component="blog-listing-cards"]', (root) => {
  const grid = root.querySelector<HTMLElement>('[data-grid]');
  if (!grid) return;

  async function loadMore(button: HTMLButtonElement) {
    button.dataset.loading = 'true';
    button.disabled = true;

    const url = button.dataset.url ?? '';
    const query = new URLSearchParams({
      pageId: button.dataset.pageId ?? '',
      skip: button.dataset.skip ?? '0',
      take: button.dataset.take ?? '3',
    });

    try {
      const res = await fetch(`${url}?${query.toString()}`, {
        headers: { 'X-Requested-With': 'fetch' },
      });
      if (!res.ok) throw new Error(`Load more failed: ${res.status}`);

      const fragment = document.createRange().createContextualFragment((await res.text()).trim());

      // The response carries the next cards and a fresh button slot (or none on
      // the last page). Drop the current button before appending so we never end
      // up with two — then the new slot, if any, lands at the bottom.
      button.closest('[data-load-more-slot]')?.remove();
      grid!.append(fragment);
    } catch (err) {
      console.error(err);
      button.dataset.loading = 'false';
      button.disabled = false;
    }
  }

  // Sync delegated handler (addEventListener wants a void return); it fires the
  // async work without awaiting.
  root.addEventListener('click', (e) => {
    const button = (e.target as Element).closest<HTMLButtonElement>('[data-load-more]');
    if (!button || button.dataset.loading === 'true') return;
    void loadMore(button);
  });
});
