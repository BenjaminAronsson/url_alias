// theme.service.ts
import { Injectable, Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { OverlayContainer } from '@angular/cdk/overlay';

export type ThemeName = 'theme-light' | 'theme-dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private _current: ThemeName = 'theme-light';
  get current() { return this._current; }

  constructor(
    @Inject(DOCUMENT) private readonly doc: Document,
    private readonly overlay: OverlayContainer
  ) {
    // initialize
    this.applyTheme(this._current);
  }

  toggle() {
    this.applyTheme(this._current === 'theme-light' ? 'theme-dark' : 'theme-light');
  }

  private applyTheme(theme: ThemeName) {
    // body
    const body = this.doc.body.classList;
    body.remove('theme-light', 'theme-dark');
    body.add(theme);

    // overlay container (menus, dialogs, **datepicker**)
    const overlay = this.overlay.getContainerElement().classList;
    overlay.remove('theme-light', 'theme-dark');
    overlay.add(theme);

    this._current = theme;
  }
}
