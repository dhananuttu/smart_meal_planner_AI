import { Link } from "react-router-dom";
import type { ReactNode } from "react";
import { EmptyState, WidgetSkeleton } from "@/components/common";

interface DashboardWidgetProps {
  title: string;
  href: string;
  isLoading: boolean;
  isError: boolean;
  isEmpty?: boolean;
  emptyMessage?: string;
  emptyAction?: ReactNode;
  className?: string;
  children: ReactNode;
}

export function DashboardWidget({
  title,
  href,
  isLoading,
  isError,
  isEmpty = false,
  emptyMessage = "Nothing here yet.",
  emptyAction,
  className = "",
  children,
}: DashboardWidgetProps) {
  return (
    <section
      className={`flex h-full flex-col rounded-2xl border border-slate-200 bg-white shadow-sm ${className}`}
    >
      <header className="flex items-center justify-between border-b border-slate-100 px-5 py-4">
        <h2 className="text-base font-semibold text-slate-900">{title}</h2>
        <Link
          to={href}
          className="text-sm font-medium text-brand-700 hover:text-brand-600"
        >
          View all
        </Link>
      </header>
      <div className="flex flex-1 flex-col px-5 py-4">
        {isLoading ? (
          <WidgetSkeleton />
        ) : isError ? (
          <p className="text-sm text-red-600">Unable to load this section.</p>
        ) : isEmpty ? (
          <EmptyState message={emptyMessage} action={emptyAction} />
        ) : (
          children
        )}
      </div>
    </section>
  );
}
