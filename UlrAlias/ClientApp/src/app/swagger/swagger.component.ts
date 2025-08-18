import { Component } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-swagger',
  templateUrl: './swagger.component.html',
  styleUrls: ['./swagger.component.scss']
})
export class SwaggerComponent {

  swaggerUrl: SafeResourceUrl;

  constructor(private readonly sanitizer: DomSanitizer) {
    const url = window.location.origin + '/swagger/index.html';
    this.swaggerUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }

}
