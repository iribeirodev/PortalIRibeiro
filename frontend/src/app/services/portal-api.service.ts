import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import type {
  ChatRequest,
  ChatResponse,
  Project,
  ProjectJson,
} from '../models/portal.models';

@Injectable({ providedIn: 'root' })
export class PortalApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  getProjects(): Observable<Project[]> {
    return this.http.get<ProjectJson[]>(`${this.baseUrl}/projects`).pipe(
      map((items) => (Array.isArray(items) ? items : []).map(toProject)),
    );
  }

  registerVisit(page = '/'): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/telemetry/visit`, { page });
  }

  sendChat(sessionId: string, text: string): Observable<ChatResponse> {
    return this.http.post<ChatResponse>(
      `${this.baseUrl}/iris/chat`,
      { sessionId, text } satisfies ChatRequest,
    );
  }
}

function toProject(raw: ProjectJson): Project {
  return {
    id: raw.id,
    title: raw.title,
    description: raw.description,
    technologies: raw.technologies,
    imageUrl: raw.imageUrl,
    githubUrl: raw.gitHubUrl,
    demoUrl: raw.demoUrl,
    createdAt: raw.createdAt,
    isActive: raw.isActive,
  };
}