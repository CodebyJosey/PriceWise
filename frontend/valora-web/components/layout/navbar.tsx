"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { LayoutDashboard, LogOut, Shield } from "lucide-react";
import { clearAccessToken } from "@/lib/auth/token-storage";
import { useCurrentUser } from "@/hooks/use-current-user";

const navigation = [
  { label: "Home", href: "/" },
  { label: "Browse", href: "/browse" },
  { label: "Features", href: "/#features" },
];

export function Navbar(): React.JSX.Element {
  const router = useRouter();
  const { user, isAuthenticated } = useCurrentUser();

  function handleLogout(): void {
    clearAccessToken();
    router.push("/");
    router.refresh();
  }

  const isAdmin = user?.roles.includes("Admin") ?? false;

  return (
    <header className="sticky top-0 z-50 border-b border-emerald-950/10 bg-white/70 backdrop-blur-xl">
      <div className="mx-auto flex h-20 w-full max-w-7xl items-center justify-between px-6 lg:px-8">
        <Link href="/" className="flex items-center gap-3">
          <div className="flex h-12 w-12 items-center justify-center rounded-[1.35rem] bg-gradient-to-br from-emerald-300 via-lime-200 to-teal-200 shadow-[0_10px_30px_rgba(110,231,183,0.45)]">
            <span className="text-lg font-black text-emerald-950">V</span>
          </div>

          <div className="flex flex-col">
            <span className="text-sm font-black uppercase tracking-[0.2em] text-emerald-950">
              Valora
            </span>
            <span className="text-xs text-emerald-900/55">
              Smart marketplace pricing
            </span>
          </div>
        </Link>

        <nav className="hidden items-center gap-8 md:flex">
          {navigation.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              className="text-sm font-semibold text-emerald-950/65 transition hover:text-emerald-950"
            >
              {item.label}
            </Link>
          ))}
        </nav>

        <div className="flex items-center gap-3">
          {isAuthenticated ? (
            <>
              <div className="hidden rounded-full bg-emerald-50 px-4 py-2 text-sm font-semibold text-emerald-950 md:block">
                {user?.displayName ?? "Ingelogd"}
              </div>

              <Link
                href="/dashboard"
                className="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white px-4 py-2 text-sm font-semibold text-emerald-950 shadow-sm transition hover:bg-emerald-50"
              >
                <LayoutDashboard className="h-4 w-4" />
                Dashboard
              </Link>

              {isAdmin ? (
                <Link
                  href="/admin"
                  className="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white px-4 py-2 text-sm font-semibold text-emerald-950 shadow-sm transition hover:bg-emerald-50"
                >
                  <Shield className="h-4 w-4" />
                  Admin
                </Link>
              ) : null}

              <button
                type="button"
                onClick={handleLogout}
                className="inline-flex items-center gap-2 rounded-full bg-emerald-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-emerald-800"
              >
                <LogOut className="h-4 w-4" />
                Uitloggen
              </button>
            </>
          ) : (
            <>
              <Link
                href="/login"
                className="hidden rounded-full border border-emerald-200 bg-white px-4 py-2 text-sm font-semibold text-emerald-950 shadow-sm transition hover:bg-emerald-50 sm:inline-flex"
              >
                Inloggen
              </Link>

              <Link
                href="/register"
                className="inline-flex rounded-full bg-emerald-900 px-4 py-2 text-sm font-semibold text-white transition hover:scale-[1.02]"
              >
                Start gratis
              </Link>
            </>
          )}
        </div>
      </div>
    </header>
  );
}