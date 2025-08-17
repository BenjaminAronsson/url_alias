import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {AliasDto} from "../models/AliasDto";

@Injectable({
  providedIn: 'root'
})
export class UrlShortenerService {
  private readonly apiUrl = '/api/aliases';

  constructor(private readonly http: HttpClient) {}

  createAlias(payload: { url: string; alias: string; expireDate: Date | null }): Observable<AliasDto> {
    return this.http.post<AliasDto>(this.apiUrl, payload);
  }
}
