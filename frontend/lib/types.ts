export interface Project {
  id: number;
  title: string;
  description: string;
  technologies: string[];
  imageUrl?: string | null;
  githubUrl?: string | null;
  demoUrl?: string | null;
  createdAt: string;
  isActive: boolean;
}

export interface ChatRequest {
  sessionId: string;
  text: string;
}

export interface ChatResponse {
  sessionId: string;
  text: string;
}

export interface ChatMessage {
  text: string;
  isUser: boolean;
  timestamp: string;
}
