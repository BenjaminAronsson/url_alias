import { Component, OnInit } from '@angular/core';
import { UrlShortenerService } from '../services/url-shortener.service';
import { AliasDto } from '../models/AliasDto';
import {PageEvent} from "@angular/material/paginator";

@Component({
  selector: 'app-inventory',
  templateUrl: './inventory.component.html',
  styleUrls: ['./inventory.component.scss']
})
export class InventoryComponent implements OnInit {
  aliases: AliasDto[] = [];
  displayedColumns: string[] = ['alias', 'url', 'clicks', 'expiresAt'];
  currentPage: number = 1;
  pageSize: number = 10;
  filter: string = '';
  totalAliases: number = 0;

  constructor(private readonly urlShortenerService: UrlShortenerService) {}

  loadAliases(): void {
    this.urlShortenerService.getAliases(this.currentPage, this.pageSize, this.filter).subscribe((response) => {
      console.log('Loaded aliases:', response);
      this.aliases = response.aliases;
      this.totalAliases = response.totalAliases;
    });
  }

  onFilterChange(filter: object): void {
    //this.filter = filter;
    this.loadAliases();
  }

  onPageChange(page: PageEvent): void {
    this.currentPage = page.pageIndex + 1;
    this.pageSize = page.pageSize;
    this.loadAliases();
  }

  ngOnInit(): void {
    this.loadAliases();
  }
}
