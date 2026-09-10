import { Component, inject, signal } from '@angular/core';
import { PortalApiService } from '../../services/portal-api.service';
import type { Project } from '../../models/portal.models';

const CACHE_TTL_MS = 3600 * 1000;

@Component({
  selector: 'app-laboratory',
  imports: [],
  templateUrl: './laboratory.html',
})
export class Laboratory {
  private readonly api = inject(PortalApiService);

  protected readonly projects = signal<Project[]>([]);
  protected readonly ready = signal(false);

  private cacheKey: string | null = null;
  private cacheUntil = 0;

  constructor() {
    void this.load();
  }

  /**
   * Carrega os projetos do laboratório, usando cache em memória de 1h para
   * evitar chamadas repetidas à API dentro da mesma sessão.
   */
  private async load(): Promise<void> {
    const now = Date.now();
    if (this.cacheKey === 'projects' && now < this.cacheUntil) {
      this.ready.set(true);
      return;
    }

    this.api.getProjects().subscribe({
      next: (data) => {
        this.projects.set(Array.isArray(data) ? data : []);
        this.cacheKey = 'projects';
        this.cacheUntil = now + CACHE_TTL_MS;
        this.ready.set(true);
      },
      error: () => {
        this.projects.set([]);
        this.ready.set(true);
      },
    });
  }
}