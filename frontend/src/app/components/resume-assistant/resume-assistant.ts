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

/**
 * Verifica se a data informada corresponde ao dia atual.
 * @param dataUso Data em formato ISO (string) armazenada no localStorage.
 * @returns `true` quando a data é hoje; caso contrário, `false`.
 */
function isToday(dataUso: string): boolean {
  const data = new Date(dataUso);
  const agora = new Date();
  return (
    data.getFullYear() === agora.getFullYear() &&
    data.getMonth() === agora.getMonth() &&
    data.getDate() === agora.getDate()
  );
}

/**
 * Retorna o timestamp atual no formato ISO 8601 (UTC).
 */
function nowIso(): string {
  return new Date().toISOString();
}

/**
 * Lê o controle de uso diário da Íris persistido no localStorage.
 * @returns O controle salvo, ou `null` quando inexistente/corrompido.
 */
function loadUsageControl(): ControleUsoIris | null {
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
      timestamp: nowIso(),
    },
  ]);
  protected readonly perguntasFeitas = signal(0);

  private sessionId: string = crypto.randomUUID();
  private streamSubscription: Subscription | null = null;

  private readonly messagesEnd = viewChild<ElementRef<HTMLDivElement>>('messagesEnd');

  constructor() {
    const controle = loadUsageControl();
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

    const atual = loadUsageControl();
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

  /**
   * Exibe a mensagem de limite diário atingido após uma pequena pausa.
   * @param perguntas Quantidade de perguntas já realizadas hoje.
   */
  private async limitReached(perguntas: number): Promise<void> {
    await new Promise((r) => setTimeout(r, 800));
    this.appendMessage(
      `Você já realizou **${perguntas} de ${LIMITE_MAXIMO} perguntas** hoje. Para garantir a disponibilidade do serviço para outros recrutadores, o limite diário foi atingido. Que tal avaliarmos mais do trabalho do Itamar direto no GitHub ou agendarmos uma conversa?`,
      false,
    );
    this.isTyping.set(false);
  }

  /**
   * Envia a mensagem à API da Íris e incorpora a resposta na conversa.
   * Ao concluir com sucesso, incrementa o contador de uso diário.
   * @param text Texto da pergunta enviada pelo usuário.
   */
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
          this.incrementUsageCounter();
        }
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
   * Incrementa o controle de uso diário no localStorage, respeitando o dia
   * corrente. Falhas de armazenamento são silenciadas.
   */
  private incrementUsageCounter(): void {
    try {
      const atual = loadUsageControl();
      let novo: ControleUsoIris;

      if (atual && isToday(atual.dataUso)) {
        novo = {
          ...atual,
          quantidadePerguntas: atual.quantidadePerguntas + 1,
        };
      } else {
        novo = { quantidadePerguntas: 1, dataUso: nowIso() };
      }

      localStorage.setItem(STORAGE_KEY, JSON.stringify(novo));
      this.perguntasFeitas.set(novo.quantidadePerguntas);
    } catch {
      // ignora falhas de armazenamento
    }
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