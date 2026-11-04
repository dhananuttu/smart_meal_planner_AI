function PlaceholderPage({ title }: { title: string }) {
  return (
    <section className="rounded-2xl border border-slate-200 bg-white p-8 shadow-sm">
      <h1 className="text-2xl font-bold text-slate-900">{title}</h1>
      <p className="mt-2 text-slate-600">Page scaffold — implement in features/</p>
    </section>
  );
}

export function ProfilePage() {
  return <PlaceholderPage title="Profile" />;
}

export function AlertsPage() {
  return <PlaceholderPage title="Inventory Alerts" />;
}
