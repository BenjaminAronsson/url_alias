import { Component } from '@angular/core';
import { UrlShortenerService } from '../services/url-shortener.service';
import {AliasDto} from "../models/AliasDto";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  url: string = '';
  alias: string = '';
  expireDate: Date | null = null;
  createdAlias: AliasDto | null = null;

  isLoading: boolean = false;
  disabled: boolean = this.isLoading || !this.url;

  constructor(private readonly urlShortenerService: UrlShortenerService) {}

  generateShortUrl() {
    this.isLoading = true;
    const payload = {
      url: this.url,
      alias: this.alias,
      expireDate: this.expireDate
    };

    this.createdAlias = null; // Reset short URL before making the request
    this.urlShortenerService.createAlias(payload).subscribe({
      next: (response) => {
        this.createdAlias = response;
        this.clearForm();
        this.isLoading = false; // Stop loading state on success
      },
      error: (error) => {
        console.error('Error generating short URL:', error);
        alert('Failed to generate short URL. Alias must be unique and URL must be valid.');
        this.isLoading = false; // Stop loading state on error
      }
    });
  }

  copyToClipboard(shortUrl: string) {
    navigator.clipboard.writeText(shortUrl).then(
      () => {},
      (error) => {
        console.error('Failed to copy short URL:', error);
        alert('Failed to copy short URL.');
      }
    );
  }

  private clearForm() {
    this.url = ''; // Clear the input field after successful creation
    this.alias = ''; // Clear the alias field after successful creation
    this.expireDate = null; // Reset expire date
    this.disabled = true; // Disable button after creation
  }
}
