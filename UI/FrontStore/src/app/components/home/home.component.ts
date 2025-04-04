import { Component } from '@angular/core';

import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms'
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ButtonModule, FormsModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  isRed: boolean = false;
  name: string = '';
}
