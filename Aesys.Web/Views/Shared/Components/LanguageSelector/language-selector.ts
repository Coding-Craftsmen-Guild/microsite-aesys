import { defineComponent } from '@/lib/component';

// The language pill is a native <details> disclosure: it opens/closes and is
// keyboard-accessible on its own, with no JS. This only adds the conveniences a
// bare <details> lacks — close when clicking outside, and close on Escape.
defineComponent<HTMLDetailsElement>('[data-component="language-selector"]', (el) => {
  const close = () => {
    el.open = false;
  };

  document.addEventListener('click', (e) => {
    if (el.open && !el.contains(e.target as Node)) close();
  });

  el.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && el.open) {
      close();
      el.querySelector<HTMLElement>('summary')?.focus();
    }
  });
});
