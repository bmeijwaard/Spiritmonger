import { FuseNavigation } from '@fuse/types';

export const navigation: FuseNavigation[] = [
    {
        id       : 'applications',
        title    : 'SE NicFit',
        type     : 'group',
        icon     : 'color_lens',
        children : [
            {
                id       : 'cards',
                title    : 'Cards',
                type     : 'item',
                icon     : 'image',
                url      : '/cards'
            }
        ]
    }
];
