import { Component, effect, inject, OnDestroy } from '@angular/core';
import { ElementRef, signal, viewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { MarkdownComponent } from 'ngx-markdown';
import { PortalApiService } from '../../services/portal-api.service';
import type { ChatMessage } from '../../models/portal.models';

/**
 * Retorna o timestamp atual no formato ISO 8601 (UTC).
 */
function nowIso(): string {
  return new Date().toISOString();
}

@Component({
  selector: 'app-resume-assistant',
  imports: [MarkdownComponent],
  templateUrl: './resume-assistant.html',
})
export class ResumeAssistant implements OnDestroy {
  private readonly api = inject(PortalApiService);

  protected readonly isChatOpen = signal(false);
  protected readonly isTyping = signal(false);
  protected readonly userInput = signal('');
  protected readonly messages = signal<ChatMessage[]>([
    {
      text: 'Olá! Eu sou a Iris, assistente inteligente do Itamar. Pode me perguntar sobre a trajetória dele, stack técnica ou experiências profissionais!',
      isUser: false,
      timestamp: nowIso(),
    },
  ]);

  private sessionId: string = crypto.randomUUID();
  private streamSubscription: Subscription | null = null;

  private readonly messagesEnd = viewChild<ElementRef<HTMLDivElement>>('messagesEnd');

  constructor() {
    effect(() => {
      this.messages();
      this.isTyping();
      this.isChatOpen();
      if (this.isChatOpen()) {
        this.scrollToBottom();
      }
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

    this.userInput.set('');
    this.appendMessage(text, true);
    this.isTyping.set(true);

    void this.streamResponse(text);
  }

  /**
   * Envia a mensagem à API da Íris e incorpora a resposta na conversa.
   * @param text Texto da pergunta enviada pelo usuário.
   */
  private async streamResponse(text: string): Promise<void> {
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
      error: (err) => {
        // A API já impõe o limite diário e o fail-closed no backend; aqui só
        // repassamos a mensagem do servidor quando ela estiver disponível.
        const serverMessage = (err as { error?: { message?: string } } | null)?.error?.message;
        this.appendMessage(
          serverMessage ?? 'Erro de conexão ao tentar falar com o servidor da Íris.',
          false,
        );
        this.isTyping.set(false);
        this.streamSubscription = null;
      },
      complete: () => {
        this.isTyping.set(false);
        this.streamSubscription = null;
      },
    });
  }

  /**
   * Acrescenta uma mensagem ao histórico visível do chat.
   * @param text Conteúdo textual da mensagem.
   * @param isUser Define se a mensagem é do usuário (`true`) ou da Íris (`false`).
   */
  private appendMessage(text: string, isUser: boolean): void {
    this.messages.update((prev) => [
      ...prev,
      { text, isUser, timestamp: nowIso() },
    ]);
  }

  /**
   * Rola a área de mensagens até o final, suavizando a animação.
   */
  private scrollToBottom(): void {
    const el = this.messagesEnd()?.nativeElement;
    if (el && typeof el.scrollIntoView === 'function') {
      requestAnimationFrame(() => el.scrollIntoView({ behavior: 'smooth' }));
    }
  }
}