import { defineComponent } from '@/lib/component';

defineComponent<HTMLElement>('[data-component="header"]', (el) => {
  const toggle = el.querySelector<HTMLButtonElement>('[data-header-toggle]');
  toggle?.addEventListener('click', () => {
    el.classList.toggle('is-open');
  });

  el.querySelectorAll<HTMLButtonElement>('[data-nav-accordion-toggle]').forEach((button) => {
    const accordion = button.closest('[data-nav-accordion]');
    const panel = accordion?.querySelector<HTMLElement>('[data-nav-accordion-panel]');
    button.addEventListener('click', () => {
      const expanded = button.getAttribute('aria-expanded') === 'true';
      button.setAttribute('aria-expanded', String(!expanded));
      button.querySelector('svg')?.classList.toggle('rotate-180', !expanded);
      panel?.classList.toggle('hidden', expanded);
      panel?.classList.toggle('flex', !expanded);
    });
  });

  const onScroll = () => {
    el.classList.toggle('scrolled', window.scrollY > 8);
  };
  onScroll();
  window.addEventListener('scroll', onScroll, { passive: true });
});
