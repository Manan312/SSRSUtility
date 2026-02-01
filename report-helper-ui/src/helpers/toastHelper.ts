import toast from "react-hot-toast"
export const toastHelper= async (message: string,type:string,time:number,icon:string): Promise<void> =>
{
    if(type=="success")
    {
        toast.success(
          message,
          {
            duration: time,
            icon: icon,
          },
        );
    }
    else
    {
        toast.error(
          message,
          {
            duration: time,
            icon: icon,
          },
        );
    }
}