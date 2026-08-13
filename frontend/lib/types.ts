export interface Projeto {
  id: number;
  titulo: string;
  descricao: string;
  tecnologias: string[];
  urlImagem?: string | null;
  urlGithub?: string | null;
  urlDemonstracao?: string | null;
  dataCriacao: string;
  ativo: boolean;
}

export interface RequisicaoChat {
  sessaoId: string;
  texto: string;
}

export interface RespostaChat {
  sessaoId: string;
  texto: string;
}

export interface ChatMessage {
  text: string;
  isUser: boolean;
  timestamp: string;
}
