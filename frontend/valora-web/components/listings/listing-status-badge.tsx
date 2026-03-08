interface ListingStatusBadgeProps {
  status: string;
}

function getStatusClasses(status: string): string {
  const normalized = status.toLowerCase();

  if (normalized === "published") {
    return "bg-emerald-100 text-emerald-900";
  }

  if (normalized === "draft") {
    return "bg-amber-100 text-amber-900";
  }

  if (normalized === "sold") {
    return "bg-sky-100 text-sky-900";
  }

  if (normalized === "archived") {
    return "bg-slate-100 text-slate-900";
  }

  return "bg-neutral-100 text-neutral-900";
}

export function ListingStatusBadge({
  status,
}: ListingStatusBadgeProps): React.JSX.Element {
  return (
    <span
      className={`inline-flex rounded-full px-3 py-1 text-xs font-bold ${getStatusClasses(
        status,
      )}`}
    >
      {status}
    </span>
  );
}