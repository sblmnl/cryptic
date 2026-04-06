import { api } from "@/boot/axios";

export async function sendDestroyNoteRequest(noteId: string, controlToken: string): Promise<void> {
  await api.delete(`/notes/${noteId}`, {
    params: { controlToken },
  });
}
