"use client";

import { useCurrentUser } from "@/hooks/use-current-user";

interface AdminHeaderProps {
  eyebrow: string;
  title: string;
  description: string;
}

export function AdminHeader({
  eyebrow,
  title,
  description,
}: AdminHeaderProps): React.JSX.Element {
  const { user } = useCurrentUser();

  return (
    <section className="soft-panel rounded-[2.2rem] p-8">
      <p className="text-sm font-bold uppercase tracking-[0.22em] text-emerald-900/50">
        {eyebrow}
      </p>

      <div className="mt-3 flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
          <h1 className="text-4xl font-black tracking-tight text-emerald-950 md:text-5xl">
            {title}
          </h1>
          <p className="mt-4 max-w-3xl text-sm leading-7 text-emerald-950/65">
            {description}
          </p>
        </div>

        <div className="rounded-[1.5rem] border border-emerald-200 bg-white px-5 py-4 shadow-sm">
          <p className="text-xs font-bold uppercase tracking-[0.18em] text-emerald-900/45">
            Admin sessie
          </p>
          <p className="mt-2 text-lg font-black text-emerald-950">
            {user?.displayName ?? "Admin"}
          </p>
          <p className="mt-1 text-sm text-emerald-950/55">
            {user?.roles.join(", ") ?? "-"}
          </p>
        </div>
      </div>
    </section>
  );
}