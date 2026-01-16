import type { NoteId } from "@/lib/models/notes/note";

export interface CreateNoteHttpRequest {
  content: string;
  deleteAfter: number;
  password?: string;
  clientMetadata?: string;
}

export interface CreateNoteHttpResponse {
  noteId: NoteId;
  controlToken: string;
}
