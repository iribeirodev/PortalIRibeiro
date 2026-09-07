import { Component, signal } from '@angular/core';

const NAV_ITEMS = [
  { id: 'hero', label: 'Home' },
  { id: 'about', label: 'Sobre' },
  { id: 'laboratory', label: 'Laboratório' },
  { id: 'services', label: 'Serviços' },
  { id: 'contacts', label: 'Contato' },
];

@Component({
  selector: 'app-navbar',
  imports: [],
  templateUrl: './navbar.html',
})
export class Navbar {
  protected readonly navItems = NAV_ITEMS;
  protected readonly open = signal(false);

  toggle(): void {
    this.open.update((current) => !current);
  }

  close(): void {
    this.open.set(false);
  }
}