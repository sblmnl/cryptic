import { api } from "@/boot/axios";
import type { OkHttpResponseBody } from "@/shared/api/http-response-body";
import type { NoteId } from "../note";

export interface ReadNoteHttpResponse {
  noteId: NoteId;
  content: string;
  destroyed: boolean;
  clientMetadata: string | null;
}

export async function sendReadNoteRequest(noteId: string, password?: string): Promise<ReadNoteHttpResponse> {
  const res = await api.post<OkHttpResponseBody<ReadNoteHttpResponse>>(`/notes/${noteId}/read`, null, {
    params: { password },
  });
  return res.data.data;
}
