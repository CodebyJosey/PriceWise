"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { LayoutDashboard, PlusCircle, Rows3 } from "lucide-react";

const navigation = [
  {
    label: "Overzicht",
    href: "/dashboard",
    icon: LayoutDashboard,
  },
  {
    label: "Mijn listings",
    href: "/dashboard/listings",
    icon: Rows3,
  },
  {
    label: "Nieuwe listing",
    href: "/dashboard/listings/new",
    icon: PlusCircle,
  },
];

export function DashboardSidebar(): React.JSX.Element {
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