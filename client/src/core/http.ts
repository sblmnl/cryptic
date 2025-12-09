import type { CodedError } from "./errors";

export interface HttpResponseBody {
  status: "ok" | "failed";
}

export interface OkHttpResponseBody<TData> extends HttpResponseBody {
  data: TData;
}

export interface FailedHttpResponseBody extends HttpResponseBody {
  errors: CodedError[];
}
