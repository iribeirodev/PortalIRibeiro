import { Component } from '@angular/core';

const SERVICES = [
  {
    title: 'Desenvolvimento',
    description: 'Aplicações web robustas com as melhores tecnologias',
    icon: '💻',
  },
  {
    title: 'Otimização',
    description: 'Performance e SEO para melhor visibilidade',
    icon: '⚡',
  },
];

@Component({
  selector: 'app-services',
  imports: [],
  templateUrl: './services.html',
})
export class Services {
  protected readonly services = SERVICES;
}