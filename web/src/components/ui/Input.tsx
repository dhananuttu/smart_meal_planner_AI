import type { InputHTMLAttributes } from "react";

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  error?: string;
}

export function Input({ className = "", error, id, ...props }: InputProps) {
  return (
    <div>
      <input
        id={id}
        className={`block w-full rounded-lg border px-3 py-2.5 text-sm shadow-sm transition focus:outline-none focus:ring-2 ${
          error
            ? "border-red-300 focus:border-red-500 focus:ring-red-200"
            : "border-slate-300 focus:border-brand-500 focus:ring-brand-200"
        } ${className}`}
        {...props}
      />
      {error ? <p className="mt-1 text-xs text-red-600">{error}</p> : null}
    </div>
  );
}
