import { AfterViewInit, Component } from '@angular/core';
import { Navbar } from './components/navbar/navbar';
import { Hero } from './components/hero/hero';
import { About } from './components/about/about';
import { Laboratory } from './components/laboratory/laboratory';
import { Services } from './components/services/services';
import { Contact } from './components/contact/contact';
import { ResumeAssistant } from './components/resume-assistant/resume-assistant';
import { Telemetry } from './components/telemetry/telemetry';

@Component({
  selector: 'app-root',
  imports: [Navbar, Hero, About, Laboratory, Services, Contact, ResumeAssistant, Telemetry],
  templateUrl: './app.html',
})
export class App implements AfterViewInit {
  constructor() {
    if ('scrollRestoration' in history) {
      history.scrollRestoration = 'manual';
    }
    this.stripUrlFragment();
  }

  private stripUrlFragment(): void {
    if (location.hash) {
      history.replaceState(null, '', location.pathname + location.search);
    }
  }

  ngAfterViewInit(): void {
    this.resetScrollToTop();
    requestAnimationFrame(() => this.resetScrollToTop());
    setTimeout(() => this.resetScrollToTop(), 300);
  }

  private resetScrollToTop(): void {
    window.scrollTo({ top: 0, left: 0, behavior: 'instant' });
  }
}