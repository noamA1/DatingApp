import { Observable, map } from 'rxjs';
import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root',
})
export class ConfirmService {
  bsModalRef?: BsModalRef<ConfirmDialogComponent>;
  constructor(private modalService: BsModalService) {}

  confirm(
    title = 'Cofirmation',
    message = 'Are you sure yoe want to do this?',
    btnOkText = 'Ok',
    btnCancelText = 'Cancel'
  ): Observable<boolean> {
    const config = {
      initialState: {
        title,
        message,
        btnOkText,
        btnCancelText,
      },
    };
    this.bsModalRef = this.modalService.show(ConfirmDialogComponent, config);
    return this.bsModalRef.onHidden!.pipe(
      map(() => {
        return this.bsModalRef!.content!.result;
      })
    );
  }
}
