import { EditListingForm } from "@/components/dashboard/edit-listing-form";

interface DashboardEditListingPageProps {
  params: Promise<{
    id: string;
  }>;
}

export default async function DashboardEditListingPage({
  params,
}: DashboardEditListingPageProps): Promise<React.JSX.Element> {
  const { id } = await params;

  return <EditListingForm listingId={id} />;
}