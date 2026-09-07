import { Component } from '@angular/core';
import { Navbar } from './components/navbar/navbar';
import { Hero } from './components/hero/hero';
import { About } from './components/about/about';
import { Laboratory } from './components/laboratory/laboratory';
import { Services } from './components/services/services';
import { Contact } from './components/contact/contact';
import { ResumeAssistant } from './components/resume-assistant/resume-assistant';
import { Telemetry } from './components/telemetry/telemetry';

@Component({
  selector: 'app-root',
  imports: [Navbar, Hero, About, Laboratory, Services, Contact, ResumeAssistant, Telemetry],
  templateUrl: './app.html',
})
export class App {}