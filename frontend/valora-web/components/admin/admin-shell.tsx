import { AdminSidebar } from "@/components/admin/admin-sidebar";

interface AdminShellProps {
  children: React.ReactNode;
}

export function AdminShell({ children }: AdminShellProps): React.JSX.Element {
  return (
    <main className="mx-auto w-full max-w-7xl px-6 py-10 lg:px-8">
      <div className="grid gap-6 lg:grid-cols-[260px_1fr]">
        <AdminSidebar />
        <div className="min-w-0">{children}</div>
      </div>
    </main>
  );
}