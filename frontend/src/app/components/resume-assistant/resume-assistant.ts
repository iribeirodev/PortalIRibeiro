import { Component, effect, inject, OnDestroy } from '@angular/core';
import { ElementRef, signal, viewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { MarkdownComponent } from 'ngx-markdown';
import { PortalApiService } from '../../services/portal-api.service';
import type { ChatMessage } from '../../models/portal.models';

const STORAGE_KEY = 'iris_usage_tracker';
const LIMITE_MAXIMO = 10;

interface ControleUsoIris {
  quantidadePerguntas: number;
  dataUso: string;
}

function isToday(dataUso: string): boolean {
  const data = new Date(dataUso);
  const agora = new Date();
  return (
    data.getFullYear() === agora.getFullYear() &&
    data.getMonth() === agora.getMonth() &&
    data.getDate() === agora.getDate()
  );
}

function agoraIso(): string {
  return new Date().toISOString();
}

function lerControle(): ControleUsoIris | null {
  try {
    const json = localStorage.getItem(STORAGE_KEY);
    if (json) {
      return JSON.parse(json) as ControleUsoIris;
    }
    return null;
  } catch {
    return null;
  }
}

@Component({
  selector: 'app-resume-assistant',
  imports: [MarkdownComponent],
  templateUrl: './resume-assistant.html',
})
export class ResumeAssistant implements OnDestroy {
  private readonly api = inject(PortalApiService);

  protected readonly LIMITE_MAXIMO = LIMITE_MAXIMO;

  protected readonly isChatOpen = signal(false);
  protected readonly isTyping = signal(false);
  protected readonly userInput = signal('');
  protected readonly messages = signal<ChatMessage[]>([
    {
      text: 'Olá! Eu sou a Iris, assistente inteligente do Itamar. Pode me perguntar sobre a trajetória dele, stack técnica ou experiências profissionais!',
      isUser: false,
      timestamp: agoraIso(),
    },
  ]);
  protected readonly perguntasFeitas = signal(0);

  private sessionId: string = crypto.randomUUID();
  private streamSubscription: Subscription | null = null;

  private readonly messagesEnd = viewChild<ElementRef<HTMLDivElement>>('messagesEnd');

  constructor() {
    const controle = lerControle();
    this.perguntasFeitas.set(
      controle && isToday(controle.dataUso) ? controle.quantidadePerguntas : 0,
    );

    effect(() => {
      this.messages();
      this.isTyping();
      this.scrollToBottom();
    });
  }

  ngOnDestroy(): void {
    this.streamSubscription?.unsubscribe();
  }

  toggleChat(): void {
    this.isChatOpen.update((open) => !open);
  }

  onInput(event: Event): void {
    this.userInput.set((event.target as HTMLInputElement).value);
  }

  sendMessage(): void {
    const text = this.userInput().trim();
    if (!text || this.isTyping()) {
      return;
    }

    const atual = lerControle();
    const perguntas =
      atual && isToday(atual.dataUso) ? atual.quantidadePerguntas : 0;
    const atingido = perguntas >= LIMITE_MAXIMO;

    this.userInput.set('');
    this.appendMessage(text, true);
    this.isTyping.set(true);

    if (atingido) {
      void this.limitReached(perguntas);
      return;
    }

    void this.streamResponse(text);
  }

  private async limitReached(perguntas: number): Promise<void> {
    await new Promise((r) => setTimeout(r, 800));
    this.appendMessage(
      `Você já realizou **${perguntas} de ${LIMITE_MAXIMO} perguntas** hoje. Para garantir a disponibilidade do serviço para outros recrutadores, o limite diário foi atingido. Que tal avaliarmos mais do trabalho do Itamar direto no GitHub ou agendarmos uma conversa?`,
      false,
    );
    this.isTyping.set(false);
  }

  private async streamResponse(text: string): Promise<void> {
    let countUsage = true;

    this.streamSubscription = this.api.sendChat(this.sessionId, text).subscribe({
      next: (response) => {
        if (response.sessionId) {
          this.sessionId = response.sessionId;
        }

        const textResponse = response.text;
        if (textResponse) {
          this.appendMessage(textResponse, false);
        }
      },
      error: () => {
        this.appendMessage('Erro de conexão ao tentar falar com o servidor da Íris.', false);
        this.isTyping.set(false);
      },
      complete: () => {
        if (countUsage) {
          this.incrementarContadorUso();
        }
        this.isTyping.set(false);
        this.streamSubscription = null;
      },
    });
  }

  private appendMessage(text: string, isUser: boolean): void {
    this.messages.update((prev) => [
      ...prev,
      { text, isUser, timestamp: agoraIso() },
    ]);
  }

  private incrementarContadorUso(): void {
    try {
      const atual = lerControle();
      let novo: ControleUsoIris;

      if (atual && isToday(atual.dataUso)) {
        novo = {
          ...atual,
          quantidadePerguntas: atual.quantidadePerguntas + 1,
        };
      } else {
        novo = { quantidadePerguntas: 1, dataUso: agoraIso() };
      }

      localStorage.setItem(STORAGE_KEY, JSON.stringify(novo));
      this.perguntasFeitas.set(novo.quantidadePerguntas);
    } catch {
      // ignora falhas de armazenamento
    }
  }

  private scrollToBottom(): void {
    const el = this.messagesEnd()?.nativeElement;
    if (el && typeof el.scrollIntoView === 'function') {
      requestAnimationFrame(() => el.scrollIntoView({ behavior: 'smooth' }));
    }
  }
}