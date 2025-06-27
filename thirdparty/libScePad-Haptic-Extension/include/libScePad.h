#ifndef SCEPADSTRUCTS_H  
#define SCEPADSTRUCTS_H  

#define SCE_OK 0  
#define SCE_PAD_ERROR_INVALID_ARG              0x80920001  
#define SCE_PAD_ERROR_INVALID_PORT             0x80920002  
#define SCE_PAD_ERROR_INVALID_HANDLE           0x80920003  
#define SCE_PAD_ERROR_ALREADY_OPENED           0x80920004  
#define SCE_PAD_ERROR_NOT_INITIALIZED          0x80920005  
#define SCE_PAD_ERROR_INVALID_LIGHTBAR_SETTING 0x80920006  
#define SCE_PAD_ERROR_DEVICE_NOT_CONNECTED     0x80920007  
#define SCE_PAD_ERROR_NO_HANDLE                0x80920008  
#define SCE_PAD_ERROR_FATAL                    0x809200FF  
#define SCE_PAD_ERROR_NOT_PERMITTED            0x80920101  
#define SCE_PAD_ERROR_INVALID_BUFFER_LENGTH    0x80920102  
#define SCE_PAD_ERROR_INVALID_REPORT_LENGTH    0x80920103  
#define SCE_PAD_ERROR_INVALID_REPORT_ID        0x80920104  
#define SCE_PAD_ERROR_SEND_AGAIN               0x80920105  

enum s_SceControllerType {
	UNKNOWN = 0,
	DUALSHOCK_4 = 1,
	DUALSENSE = 2
};

#pragma pack(push, 1)  
struct s_ScePadContainerIdInfo {  
uint32_t size;  
char id[0x2000]; // UTF-8 string
};  
#pragma pack(pop)  

#ifdef __cplusplus  
extern "C" {  
#endif  

int scePadGetControllerBusType(int handle, int* busType);  
int scePadGetContainerIdInformation(int handle, s_ScePadContainerIdInfo* containerIdInfo);  
int scePadGetControllerType(int handle, s_SceControllerType* controllerType);  

#ifdef __cplusplus  
}  
#endif  

#endif // SCEPADSTRUCTS_H