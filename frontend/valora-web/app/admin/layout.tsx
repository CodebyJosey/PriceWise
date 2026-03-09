import { AdminGuard } from "@/components/auth/admin-guard";
import { AdminShell } from "@/components/admin/admin-shell";

interface AdminLayoutProps {
  children: React.ReactNode;
}

export default function AdminLayout({
  children,
}: AdminLayoutProps): React.JSX.Element {
  return (
    <AdminGuard>
      <AdminShell>{children}</AdminShell>
    </AdminGuard>
  );
}