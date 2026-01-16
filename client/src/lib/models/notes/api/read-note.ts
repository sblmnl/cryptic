import type { NoteId } from "@/lib/models/notes/note";

export interface ReadNoteHttpResponse {
  noteId: NoteId;
  content: string;
  destroyed: boolean;
  clientMetadata: string | null;
}
