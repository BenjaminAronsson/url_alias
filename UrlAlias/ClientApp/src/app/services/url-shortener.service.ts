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

  getAliases(page: number = 1, pageSize: number = 10, filter: string = ''): Observable<{ aliases: AliasDto[], totalAliases: number }> {
    const params = { page: page.toString(), pageSize: pageSize.toString(), filter };
    return this.http.get<{ aliases: AliasDto[], totalAliases: number }>(this.apiUrl, { params });
  }
}
