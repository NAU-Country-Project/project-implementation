import { Home } from "./components/Home";
import { OpenPDF } from "./components/OpenPDF";
import { DownloadPDF } from "./components/DownloadPDF";

const AppRoutes = [
  // {
  //   index: true,
  //   element: <Home />
  // },
  {
    index: true,
    element: <OpenPDF />
  },
  {
    path: '/pdf-generator',
    element: <DownloadPDF />
  }
];

export default AppRoutes;
