import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private snack = inject(MatSnackBar);

  get isSupported(): boolean {
    return typeof window !== 'undefined' && 'Notification' in window;
  }

  async requestPermissionIfNeeded(): Promise<boolean> {
    if (!this.isSupported) return false;
    if (Notification.permission === 'granted') return true;
    if (Notification.permission === 'denied') return false;
    try {
      const res = await Notification.requestPermission();
      return res === 'granted';
    } catch {
      return false;
    }
  }

  notify(title: string, body?: string): void {
    if (this.isSupported && Notification.permission === 'granted') {
      try {
        new Notification(title, body ? { body } : undefined);
        return;
      } catch {
        // fallback to snackbar below
      }
    }
    // Fallback snackbar
    this.snack.open(body ? `${title}: ${body}` : title, 'Close', { duration: 2500 });
  }
}

