import { Component, OnInit } from '@angular/core';
import { AliasService } from './alias.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  alias = '';
  url = '';
  aliases: Record<string, string> = {};

  constructor(private service: AliasService) {}

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.service.list().subscribe(data => this.aliases = data);
  }

  add() {
    if (!this.alias || !this.url) return;
    this.service.add(this.alias, this.url).subscribe(() => {
      this.alias = '';
      this.url = '';
      this.refresh();
    });
  }
}
