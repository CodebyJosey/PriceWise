"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  Activity,
  BarChart3,
  LayoutDashboard,
  ScrollText,
} from "lucide-react";

const navigation = [
  {
    label: "Overzicht",
    href: "/admin",
    icon: LayoutDashboard,
  },
  {
    label: "Model performance",
    href: "/admin/model-performance",
    icon: Activity,
  },
  {
    label: "Audit logs",
    href: "/admin/audit-logs",
    icon: ScrollText,
  },
  {
    label: "Model stats",
    href: "/admin/models",
    icon: BarChart3,
  },
];

export function AdminSidebar(): React.JSX.Element {
  const pathname = usePathname();

  return (
    <aside className="soft-panel rounded-[2rem] p-4">
      <nav className="space-y-2">
        {navigation.map((item) => {
          const Icon = item.icon;
          const isActive = pathname === item.href;

          return (
            <Link
              key={item.href}
              href={item.href}
              className={`flex items-center gap-3 rounded-[1.25rem] px-4 py-3 text-sm font-semibold transition ${
                isActive
                  ? "bg-emerald-900 text-white"
                  : "text-emerald-950 hover:bg-emerald-50"
              }`}
            >
              <Icon className="h-4 w-4" />
              {item.label}
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}