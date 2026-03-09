"use client";

import { useQuery } from "@tanstack/react-query";
import { RefreshCcw } from "lucide-react";
import { AdminHeader } from "@/components/admin/admin-header";
import { adminApi } from "@/lib/api/admin-api";
import { getAccessToken } from "@/lib/auth/token-storage";

function formatDate(value: string): string {
  return new Intl.DateTimeFormat("nl-NL", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}

export function AdminAuditLogs(): React.JSX.Element {
  const token = getAccessToken();

  const logsQuery = useQuery({
    queryKey: ["admin-audit-logs"],
    queryFn: ({ signal }) => adminApi.getAuditLogs(token as string, signal),
    enabled: Boolean(token),
  });

  return (
    <div className="space-y-6">
      <AdminHeader
        eyebrow="Admin"
        title="Audit logs"
        description="Bekijk recente platformevents voor debugging, monitoring en beheer."
      />

      <section className="soft-panel rounded-[2rem] p-6">
        <div className="flex items-center justify-between gap-4">
          <div>
            <h2 className="text-2xl font-black text-emerald-950">Recente events</h2>
            <p className="mt-2 text-sm leading-7 text-emerald-950/62">
              Laatste {logsQuery.data?.count ?? 0} logs uit de backend.
            </p>
          </div>

          <button
            type="button"
            onClick={() => void logsQuery.refetch()}
            className="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white px-4 py-2 text-sm font-semibold text-emerald-950 transition hover:bg-emerald-50"
          >
            <RefreshCcw className="h-4 w-4" />
            Vernieuwen
          </button>
        </div>

        {logsQuery.isLoading ? (
          <p className="mt-6 text-sm text-emerald-950/60">Laden...</p>
        ) : null}

        {logsQuery.isError ? (
          <p className="mt-6 text-sm text-emerald-950/60">
            Audit logs konden niet worden geladen.
          </p>
        ) : null}

        {logsQuery.data ? (
          <div className="mt-6 overflow-x-auto">
            <table className="min-w-full border-separate border-spacing-y-3">
              <thead>
                <tr className="text-left text-xs uppercase tracking-[0.18em] text-emerald-900/45">
                  <th className="px-4">Actie</th>
                  <th className="px-4">User</th>
                  <th className="px-4">Entity</th>
                  <th className="px-4">Details</th>
                  <th className="px-4">Tijd</th>
                </tr>
              </thead>
              <tbody>
                {logsQuery.data.logs.map((log) => (
                  <tr key={log.id} className="bg-white shadow-sm">
                    <td className="rounded-l-[1rem] px-4 py-4 font-semibold text-emerald-950">
                      {log.action}
                    </td>
                    <td className="px-4 py-4 text-sm text-emerald-950/70">
                      {log.userId ?? "-"}
                    </td>
                    <td className="px-4 py-4 text-sm text-emerald-950/70">
                      {log.entityType}
                      {log.entityId ? ` · ${log.entityId}` : ""}
                    </td>
                    <td className="px-4 py-4 text-sm text-emerald-950/70">
                      {log.details ?? "-"}
                    </td>
                    <td className="rounded-r-[1rem] px-4 py-4 text-sm text-emerald-950/70">
                      {formatDate(log.createdAtUtc)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : null}
      </section>
    </div>
  );
}