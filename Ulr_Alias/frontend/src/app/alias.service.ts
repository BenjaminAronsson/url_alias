import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface AliasEntry { alias: string; url: string; }

@Injectable()
export class AliasService {
  private base = '/api/aliases';
  constructor(private http: HttpClient) {}

  list() {
    return this.http.get<Record<string, string>>(this.base);
  }

  add(alias: string, url: string) {
    return this.http.post<AliasEntry>(this.base, { alias, url });
  }

  get(alias: string) {
    return this.http.get<string>(`${this.base}/${alias}`);
  }
}
