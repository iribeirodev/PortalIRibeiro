import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { PortalApiService } from '../../services/portal-api.service';

@Component({
  selector: 'app-telemetry',
  imports: [],
  template: '',
})
export class Telemetry implements OnInit {
  private readonly api = inject(PortalApiService);
  private lastFired: string | null = null;

  ngOnInit(): void {
    const page = '/';
    if (this.lastFired === page) {
      return;
    }
    this.lastFired = page;
    this.api.registerVisit(page).subscribe({ error: () => undefined });
  }
}