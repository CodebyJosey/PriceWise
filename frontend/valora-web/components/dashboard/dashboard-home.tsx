"use client";

import Link from "next/link";
import { PlusCircle, Rows3, Sparkles } from "lucide-react";
import { DashboardHeader } from "@/components/dashboard/dashboard-header";
import { useCurrentUser } from "@/hooks/use-current-user";

export function DashboardHome(): React.JSX.Element {
  const { user } = useCurrentUser();

  return (
    <div className="space-y-6">
      <DashboardHeader
        eyebrow="Dashboard"
        title={`Welkom${user ? `, ${user.displayName}` : ""}`}
        description="Hier start de seller flow van Valora. Vanuit hier beheer je je listings, maak je nieuwe listings aan en werk je straks met AI price predictions."
      />

      <section className="grid gap-4 md:grid-cols-3">
        <Link
          href="/dashboard/listings"
          className="soft-panel rounded-[1.75rem] p-6 transition hover:-translate-y-0.5"
        >
          <div className="flex h-12 w-12 items-center justify-center rounded-[1.1rem] bg-emerald-100">
            <Rows3 className="h-5 w-5 text-emerald-900" />
          </div>
          <h2 className="mt-4 text-xl font-black text-emerald-950">Mijn listings</h2>
          <p className="mt-3 text-sm leading-7 text-emerald-950/62">
            Bekijk drafts en gepubliceerde listings en beheer hun status.
          </p>
        </Link>

        <Link
          href="/dashboard/listings/new"
          className="soft-panel rounded-[1.75rem] p-6 transition hover:-translate-y-0.5"
        >
          <div className="flex h-12 w-12 items-center justify-center rounded-[1.1rem] bg-lime-100">
            <PlusCircle className="h-5 w-5 text-lime-900" />
          </div>
          <h2 className="mt-4 text-xl font-black text-emerald-950">Nieuwe listing</h2>
          <p className="mt-3 text-sm leading-7 text-emerald-950/62">
            Maak een nieuwe listing aan met categorie-specifieke features.
          </p>
        </Link>

        <div className="soft-panel rounded-[1.75rem] p-6">
          <div className="flex h-12 w-12 items-center justify-center rounded-[1.1rem] bg-cyan-100">
            <Sparkles className="h-5 w-5 text-cyan-900" />
          </div>
          <h2 className="mt-4 text-xl font-black text-emerald-950">AI pricing</h2>
          <p className="mt-3 text-sm leading-7 text-emerald-950/62">
            In de volgende stap koppelen we prediction en publish slimmer in je flow.
          </p>
        </div>
      </section>
    </div>
  );
}