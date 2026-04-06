import { api } from "@/boot/axios";
import type { OkHttpResponseBody } from "@/shared/api/http-response-body";
import type { NoteId } from "../note";

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

export async function sendCreateNoteRequest(body: CreateNoteHttpRequest): Promise<CreateNoteHttpResponse> {
  const res = await api.post<OkHttpResponseBody<CreateNoteHttpResponse>>("/notes", body);
  return res.data.data;
}
