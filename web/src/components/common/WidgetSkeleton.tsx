interface WidgetSkeletonProps {
  rows?: number;
}

export function WidgetSkeleton({ rows = 3 }: WidgetSkeletonProps) {
  return (
    <div className="space-y-3" aria-hidden="true">
      {Array.from({ length: rows }).map((_, index) => (
        <div key={index} className="h-10 animate-pulse rounded-lg bg-slate-100" />
      ))}
    </div>
  );
}
