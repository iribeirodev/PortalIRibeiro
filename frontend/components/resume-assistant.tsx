"use client";

import {
  useCallback,
  useEffect,
  useReducer,
  useRef,
  useState,
  useSyncExternalStore,
} from "react";
import { chatWithIris } from "@/lib/api";
import { Markdown } from "@/lib/markdown";
import type { ChatMessage } from "@/lib/types";

const STORAGE_KEY = "iris_usage_tracker";
const LIMITE_MAXIMO = 10;

interface ControleUsoIris {
  quantidadePerguntas: number;
  dataUso: string;
}

const GREETING =
  "Olá! Eu sou a Iris, assistente inteligente do Itamar. Pode me perguntar sobre a trajetória dele, stack técnica ou experiências profissionais!";

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
    if (json) return JSON.parse(json) as ControleUsoIris;
    return null;
  } catch {
    return null;
  }
}

let controleCache: ControleUsoIris | null = null;

function getSnapshot(): ControleUsoIris | null {
  const atual = lerControle();
  if (atual === null) {
    controleCache = null;
    return null;
  }
  if (
    controleCache === null ||
    controleCache.quantidadePerguntas !== atual.quantidadePerguntas ||
    controleCache.dataUso !== atual.dataUso
  ) {
    controleCache = atual;
  }
  return controleCache;
}

function getServerSnapshot(): ControleUsoIris | null {
  return null;
}

function subscribe(): () => void {
  return () => {};
}

export function ResumeAssistant() {
  const [isChatOpen, setIsChatOpen] = useState(false);
  const [isTyping, setIsTyping] = useState(false);
  const [userInput, setUserInput] = useState("");
  const [messages, setMessages] = useState<ChatMessage[]>(() => [
    { text: GREETING, isUser: false, timestamp: agoraIso() },
  ]);
  const [, forceRender] = useReducer((x: number) => x + 1, 0);

  const sessionIdRef = useRef<string>(crypto.randomUUID());
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const controle = useSyncExternalStore(
    subscribe,
    getSnapshot,
    getServerSnapshot
  );
  const perguntasFeitas =
    controle && isToday(controle.dataUso) ? controle.quantidadePerguntas : 0;

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages, isTyping]);

  const toggleChat = () => setIsChatOpen((open) => !open);

  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      sendMessage();
    }
  };

  const incrementarContadorUso = useCallback(() => {
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
      forceRender();
    } catch {
      // ignora falhas de armazenamento
    }
  }, []);

  const sendMessage = async () => {
    if (!userInput.trim() || isTyping) return;

    const atual = lerControle();
    const perguntas =
      atual && isToday(atual.dataUso) ? atual.quantidadePerguntas : 0;
    const atingido = perguntas >= LIMITE_MAXIMO;

    const userText = userInput;
    setUserInput("");

    setMessages((prev) => [
      ...prev,
      { text: userText, isUser: true, timestamp: agoraIso() },
    ]);
    setIsTyping(true);

    if (atingido) {
      await new Promise((r) => setTimeout(r, 800));
      setMessages((prev) => [
        ...prev,
        {
          text: `Você já realizou **${perguntas} de ${LIMITE_MAXIMO} perguntas** hoje. Para garantir a disponibilidade do serviço para outros recrutadores, o limite diário foi atingido. Que tal avaliarmos mais do trabalho do Itamar direto no GitHub ou agendarmos uma conversa?`,
          isUser: false,
          timestamp: agoraIso(),
        },
      ]);
      setIsTyping(false);
      return;
    }

    try {
      const result = await chatWithIris(sessionIdRef.current, userText);

      if (result) {
        sessionIdRef.current = result.sessionId;
        setMessages((prev) => [
          ...prev,
          { text: result.text, isUser: false, timestamp: agoraIso() },
        ]);
        incrementarContadorUso();
      } else {
        setMessages((prev) => [
          ...prev,
          {
            text: "Ops, tive um problema para obter uma resposta da Íris. Tente novamente!",
            isUser: false,
            timestamp: agoraIso(),
          },
        ]);
      }
    } catch {
      setMessages((prev) => [
        ...prev,
        {
          text: "Erro de conexão ao tentar falar com o servidor da Íris.",
          isUser: false,
          timestamp: agoraIso(),
        },
      ]);
    } finally {
      setIsTyping(false);
    }
  };

  return (
    <>
      <button
        className="chat-floating-btn"
        onClick={toggleChat}
        title="Fale com a Iris"
      >
        <div className="assistant-branding">
          <img
            src="/images/Iris.png"
            alt="Iris"
            className="assistant-avatar"
          />
          <span className="assistant-label">Fale com a Iris</span>
        </div>
      </button>

      <div className={`chat-window ${isChatOpen ? "open" : ""}`}>
        <div className="chat-header">
          <div className="chat-header-title">
            <div className="assistant-branding-header">
              <img
                src="/images/Iris.png"
                alt="Iris"
                className="assistant-avatar-small"
              />
              <h5>Iris</h5>
            </div>
            <div className="header-actions">
              <span className="online-indicator"></span>
              <span className="online-text">
                Online ({perguntasFeitas}/{LIMITE_MAXIMO})
              </span>
            </div>
          </div>
        </div>

        <div className="chat-body">
          <div className="messages-container">
            {messages.map((msg, i) => (
              <div
                key={i}
                className={`message-row ${msg.isUser ? "user-row" : "iris-row"}`}
              >
                {!msg.isUser && (
                  <img
                    src="/images/Iris.png"
                    className="chat-msg-avatar"
                    alt="Iris"
                  />
                )}
                <div className="message-bubble">
                  {msg.isUser ? msg.text : <Markdown content={msg.text} />}
                </div>
              </div>
            ))}

            {isTyping && (
              <div className="message-row iris-row">
                <img
                  src="/images/Iris.png"
                  className="chat-msg-avatar"
                  alt="Iris"
                />
                <div className="message-bubble typing-bubble">
                  <span className="dot"></span>
                  <span className="dot"></span>
                  <span className="dot"></span>
                </div>
              </div>
            )}

            <div ref={messagesEndRef} />
          </div>
        </div>

        <div className="chat-footer-container">
          <div className="chat-footer">
            <input
              type="text"
              placeholder="Pergunte sobre o currículo do Itamar..."
              value={userInput}
              onChange={(e) => setUserInput(e.target.value)}
              onKeyDown={handleKeyPress}
              maxLength={100}
              disabled={isTyping}
            />
            <button
              onClick={sendMessage}
              disabled={!userInput.trim() || isTyping}
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="18"
                height="18"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
              >
                <line x1="22" y1="2" x2="11" y2="13"></line>
                <polygon points="22 2 15 22 11 13 2 9 22 2"></polygon>
              </svg>
            </button>
          </div>
          <div
            className={`char-counter ${userInput.length >= 90 ? "counter-warn" : ""}`}
          >
            {userInput.length} / 100
          </div>
        </div>
      </div>
    </>
  );
}
