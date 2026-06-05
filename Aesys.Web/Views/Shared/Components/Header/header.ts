import { defineComponent } from '@/lib/component';

defineComponent<HTMLElement>('[data-component="header"]', (el) => {
  const toggle = el.querySelector<HTMLButtonElement>('[data-header-toggle]');
  toggle?.addEventListener('click', () => {
    el.classList.toggle('is-open');
  });

  const onScroll = () => {
    el.classList.toggle('scrolled', window.scrollY > 8);
  };
  onScroll();
  window.addEventListener('scroll', onScroll, { passive: true });
});
